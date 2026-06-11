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

                    StreamReader streamReader = new StreamReader(stream);
                     File.WriteAllText(document.FullPath,
                         streamReader.ReadToEnd(),Encoding.UTF8);
                    streamReader.Close();
                    FileCopier.CopyFolderToDownloads(document.ParentDirectory,
                          Path.GetFileNameWithoutExtension(savePath),document);
                    File.Delete(savePath);
                    if (Path.GetExtension(savePath).ToLower() == StaticVariables.SevenZippedSlimeMarkDown)
                    {
                        Slime7z.Create(document.ParentDirectory,
                            Path.Combine(document.ParentDirectory,
                        Path.
                        GetFileNameWithoutExtension(savePath)
                        , StaticVariables.SevenZippedSlimeMarkDown));
                        foreach (string file in Directory.GetFiles(document.
                        ParentDirectory))
                        {
                            if (Path.GetExtension(file).ToLower() != StaticVariables.SevenZippedSlimeMarkDown)
                            {
                                File.Delete(file);
                            }
                        }
                    }
#endif
                        // }


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
