using SlimeWrite.Core.Archive;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Models;
using SlimeWrite.Core.SDK;
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
                if ( String.IsNullOrEmpty(name) )
                {
                    name = "NewDocument";
                }
                if (Path.HasExtension(name))
                {
                    string ext = Path.GetExtension(name);
                    ap.Name = name;
                    string path = Path.Combine(StaticVariables.core.
                        GetTempfolderPath(),
                        Path.GetFileNameWithoutExtension(name));
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
                    string path = Path.Combine(StaticVariables.core.
                        GetTempfolderPath(),
                    name);
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
        public void AddFileToDocumentParentDirectory(DocumentInfo document,
            string filePath)
        {
            try
            {

                string fileName = Path.GetFileName(filePath);
                string destinationPath = Path.Combine(document.ParentDirectory, 
                    fileName);
                File.Copy(filePath, destinationPath, true);
                foreach(var plugin in PluginManager.Plugins)
                {
                    plugin.AddFileToDocumentParentDirectory(document,
                        destinationPath);
                }
            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);

               
            }

        }
        public void SaveDocument(DocumentInfo document, 
            string savePath,Stream stream)
        {

            try
            {
#if WINDOWS
                if (Directory.Exists(document.ParentDirectory)
                && stream != null)
                {
                    string destinationPath = Path.Combine(
                        Path.GetDirectoryName(savePath),
                        Path.GetFileNameWithoutExtension(savePath));

                    Directory.CreateDirectory(destinationPath);

                    string destinationFile = Path.Combine(destinationPath,
                    Path.GetFileName(savePath));

                    var files = Directory.
                    GetFiles(document.ParentDirectory);
                    StreamReader streamReader = new StreamReader(stream);
                    var filecont = streamReader.ReadToEnd();
                    File.WriteAllText(Path.Combine(destinationPath,
                        Path.GetFileNameWithoutExtension(savePath) + ".smd"),
                        filecont
                        , Encoding.UTF8);
                    streamReader.Close();
                    if (StaticVariables.core.isDesktopMode())
                    {

                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);

                            if (Path.GetExtension(file).ToLower() != ".html")
                            {
                                string destFile = Path.
                                Combine(destinationPath,
                                fileName);
                                File.Move(file, destFile, true); // ή Copy αν θες αντίγραφα
                            }
                        }
                    }
                    document.FullPath = destinationFile;
                    document.ParentDirectory = destinationPath;
                    document.Name = Path.GetFileName(destinationFile);
                    if (Path.GetExtension(savePath).ToLower()
                        == StaticVariables.SevenZippedSlimeMarkDown)
                    {
                        var zippedfile = Path.Combine(document.ParentDirectory,
                     Path.
                     GetFileNameWithoutExtension(savePath)
                     , StaticVariables.SevenZippedSlimeMarkDown);
                        Slime7z.Create(document.ParentDirectory,
                          savePath);
                        foreach (string file in Directory.GetFiles(document.
                        ParentDirectory))
                        {
                            if (Path.GetExtension(file).ToLower() != StaticVariables.SevenZippedSlimeMarkDown)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                    
                }
#endif
                    // else
                    {

#if ANDROID
                    // 1. Δημιουργούμε έναν προσωρινό φάκελο στην Cache της εφαρμογής όπου το System.IO δουλεύει 100%
                    string tempCacheFolder = 
                        Path.Combine(Android.App.Application.Context.CacheDir
                        .AbsolutePath, "SaveTemp");
                    if (Directory.Exists(tempCacheFolder))
                        Directory.Delete(tempCacheFolder, true);
                    Directory.CreateDirectory(tempCacheFolder);

                    // 2. Διαβάζουμε το stream και γράφουμε το αρχικό αρχείο κειμένου τοπικά στην Cache
                    string tempTxtFile = Path.Combine(tempCacheFolder, "document.txt");
                    stream.Position = 0;
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        File.WriteAllText(tempTxtFile, streamReader.ReadToEnd(),
                            Encoding.UTF8);
                    }

                    // 3. Αν είναι 7z, φτιάχνουμε το zip μέσα στην Cache
                    string finalLocalFile = tempTxtFile;
                    if (Path.GetExtension(savePath).ToLower() 
                        == StaticVariables.SevenZippedSlimeMarkDown)
                    {
                        string tempZipFile = Path.Combine(tempCacheFolder,
                            "archive" + StaticVariables.SevenZippedSlimeMarkDown);

                        // Καλούμε το Slime7z χρησιμοποιώντας ΜΟΝΟ τοπικά, έγκυρα paths της Cache
                        Slime7z.Create(tempCacheFolder, tempZipFile);
                        finalLocalFile = tempZipFile;
                    }

                    // 4. Τώρα που έχουμε το τελικό έτοιμο αρχείο τοπικά, χρησιμοποιούμε τον ContentResolver 
                    //    του Android για να το γράψουμε στη διαδρομή (URI) που επέλεξε ο χρήστης.
                    try
                    {
                        var context = Android.App.Application.Context;
                        var androidUri = Android.Net.Uri.Parse(savePath); // Το savePath είναι το content:// URI

                        using (var outputStream = context.ContentResolver.
                            OpenOutputStream(androidUri))
                        using (var localFileStream = File.OpenRead(finalLocalFile))
                        {
                            if (outputStream != null)
                            {
                                localFileStream.CopyTo(outputStream); // Αντιγραφή των bytes στο Android Storage
                            }
                        }

                        // Ενημερώνουμε το document info
                        document.FullPath = savePath;
                        document.Name = Path.GetFileName(savePath);
                    }
                    catch (Exception ex)
                    {
                        StaticVariables.core.ErrorLog(new Exception("Android ContentResolver Error: " + ex.Message));
                    }
                    finally
                    {
                        // Καθαρίζουμε τον προσωρινό φάκελο Cache
                        if (Directory.Exists(tempCacheFolder))
                            Directory.Delete(tempCacheFolder, true);
                    }
#endif


                }

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
                    Directory.Delete(Path.Combine(document.ParentDirectory, 
                        "output.html"), true);
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
