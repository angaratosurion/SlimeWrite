using SharpCompress.Common;
using SlimeWrite.Core.Archive;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Models;
using SlimeWrite.Core.SDK;
using System;
using System.IO;
using System.Text;

namespace SlimeWrite.Core
{
    public class DocumentManager
    {
        public DocumentInfo CreateNewDocument(string name)
        {
            try
            {
                DocumentInfo ap = new DocumentInfo();
                if (String.IsNullOrEmpty(name))
                {
                    name = "NewDocument";
                }
                if (Path.HasExtension(name))
                {
                    string ext = Path.GetExtension(name);
                    ap.Name = name;
                    string path = Path.Combine(StaticVariables.core.GetTempfolderPath(), Path.GetFileNameWithoutExtension(name));
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        Directory.CreateDirectory(path);
                    }
                    ap.ParentDirectory = path;
                    ap.FullPath = Path.Combine(path, name + "." + ext);
                }
                else
                {
                    ap.Name = name;
                    string path = Path.Combine(StaticVariables.core.GetTempfolderPath(), name);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        Directory.CreateDirectory(path);
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                    }
                    ap.ParentDirectory = path;
                    ap.FullPath = Path.Combine(path, name + ".smd");
                }

                return ap;
            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);
                return null;
            }
        }

        public void AddFileToDocumentParentDirectory(DocumentInfo document, string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                string destinationPath = Path.Combine(document.ParentDirectory, fileName);
                File.Copy(filePath, destinationPath, true);
                foreach (var plugin in PluginManager.Plugins)
                {
                    plugin.AddFileToDocumentParentDirectory(document, destinationPath);
                }
            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);
            }
        }

        public async void SaveDocument(DocumentInfo document, string savePath, Stream stream)
        {
            try
            {
                ProgressReport progressReport;
                Progress<ProgressReport> progress = new Progress<ProgressReport>(report =>
                {

                    progressReport = report;
                    Console.WriteLine($"Extracting {report.EntryPath}: {report.PercentComplete}%");
                     
                });

#if WINDOWS
                if (Directory.Exists(document.ParentDirectory) && stream != null)
                {
                    string destinationPath = Path.Combine(
                        Path.GetDirectoryName(savePath),
                        Path.GetFileNameWithoutExtension(savePath));

                    Directory.CreateDirectory(destinationPath);

                    string destinationFile = Path.Combine(destinationPath, Path.GetFileName(savePath));

                    var files = Directory.GetFiles(document.ParentDirectory);
                    StreamReader winStreamReader = new StreamReader(stream); // Αλλαγή ονόματος για να μην χτυπάει CS0136
                    var filecont = winStreamReader.ReadToEnd();
                    File.WriteAllText(Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(savePath) + ".smd"), filecont, Encoding.UTF8);
                    winStreamReader.Close();

                    if (StaticVariables.core.isDesktopMode())
                    {
                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);

                            if (Path.GetExtension(file).ToLower() != ".html")
                            {
                                string destFile = Path.Combine(destinationPath, fileName);
                                File.Move(file, destFile, true);
                            }
                        }
                    }
                    document.FullPath = destinationFile;
                    document.ParentDirectory = destinationPath;
                    document.Name = Path.GetFileName(destinationFile);
                    if (Path.GetExtension(savePath).ToLower() == StaticVariables.SevenZippedSlimeMarkDown)
                    {
                        var zippedfile = Path.Combine(document.ParentDirectory, Path.GetFileNameWithoutExtension(savePath), StaticVariables.SevenZippedSlimeMarkDown);
                       await  Slime7z.Create(document.ParentDirectory, savePath,progress);
                        foreach (string file in Directory.GetFiles(document.ParentDirectory))
                        {
                            if (Path.GetExtension(file).ToLower() != StaticVariables.SevenZippedSlimeMarkDown)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }
#endif

#if ANDROID
                // 1. Δημιουργούμε έναν προσωρινό φάκελο στην Cache της εφαρμογής όπου το System.IO δουλεύει 100%
                string tempCacheFolder = Path.Combine(StaticVariables.core.
                    GetTempfolderPath(),savePath );
                if (Directory.Exists(tempCacheFolder))
                    Directory.Delete(tempCacheFolder, true);
                Directory.CreateDirectory(tempCacheFolder);

                // 2. Διαβάζουμε το stream και γράφουμε το αρχικό αρχείο κειμένου τοπικά στην Cache
                string tempTxtFile = document.FullPath;
                stream.Position = 0;
                using (StreamReader androidStreamReader = new StreamReader(stream)) // Αλλαγή ονόματος εδώ
                {
                    File.WriteAllText(tempTxtFile, androidStreamReader.ReadToEnd(),
                        Encoding.UTF8);
                    androidStreamReader.Close();
                    stream.Close();
                }

                // 3. Αν είναι 7z, φτιάχνουμε το zip μέσα στην Cache
                string finalLocalFile = tempTxtFile;
                if (Path.GetExtension(savePath).ToLower()
                    == StaticVariables.SevenZippedSlimeMarkDown)
                {
                    string tempZipFile = Path.Combine(tempCacheFolder,
                        Path.GetFileNameWithoutExtension(savePath) +
                        StaticVariables.SevenZippedSlimeMarkDown);

                    try
                    {
                        await Slime7z.Create(document.ParentDirectory, tempZipFile
                            ,progress);
                    }
                    catch (Exception ex)
                    {
                        // This will print the actual error message and the file path if available
                        Console.WriteLine($"Archiving failed: {ex.Message}");
                        Console.WriteLine(ex.ToString());
                        StaticVariables.core.ErrorLog(ex);
                    }
                    finalLocalFile = tempZipFile;
                    
                    
                    FileCopier.CopyFileToDownloads(tempZipFile,
                        tempZipFile);
                }
                else
                {
                    using (StreamReader androidStreamReader = new StreamReader(stream)) // Αλλαγή ονόματος εδώ
                    {
                        File.WriteAllText(tempTxtFile, androidStreamReader.ReadToEnd(),
                            Encoding.UTF8);
                        androidStreamReader.Close();
                        stream.Close();
                    }
                    FileCopier.CopyFolderToDownloads(tempCacheFolder, 
                        Path.GetFileNameWithoutExtension(savePath), document);
                }

                
                document.FullPath = savePath;
                document.Name = Path.GetFileName(savePath);
#endif
            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);
            }
        }

        public void CloseDocument(DocumentInfo document)
        {
            try
            {
                if (Directory.Exists(document.ParentDirectory))
                {
                    // Προσοχή: Αν το output.html είναι αρχείο, το Directory.Delete θα χτυπήσει error. 
                    // Αν είναι αρχείο, χρησιμοποίησε File.Delete(Path.Combine(...))
                    Directory.Delete(Path.Combine(document.ParentDirectory, "output.html"), true);
                }
                document = null;
            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);
            }
        }
    }
}