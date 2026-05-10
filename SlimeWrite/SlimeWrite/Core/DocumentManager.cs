using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Models;

namespace SlimeWrite.Core
{
    public class DocumentManager
    {
        
        public DocumentInfo CreateNewDocument(string name)
        {
            try
            {
                DocumentInfo ap = new DocumentInfo();
                if (Path.HasExtension(name))
                {
                    string ext = Path.GetExtension(name);
                    ap.Name = name;
                    string path = Path.Combine(MainPage.core.GetTempfolderPath(),
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
                    string path = Path.Combine(MainPage.core.GetTempfolderPath(),
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
                MainPage.core.ErrorLog(ex);
                
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
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

               
            }

        }
        public void SaveDocument(DocumentInfo document, string savePath,Stream stream)
        {

            try
            {
#if WINDOWS
                if (Directory.Exists(document.ParentDirectory) && stream != null)
                {
                    string destinationPath = Path.Combine(
                        Path.GetDirectoryName(savePath),
                        Path.GetFileNameWithoutExtension(savePath));

                    Directory.CreateDirectory(destinationPath);

                    string destinationFile = Path.Combine(destinationPath, Path.GetFileName(savePath));
                    File.Move(savePath, destinationFile, true);

                    var files = Directory.GetFiles(document.ParentDirectory);
                    if (MainPage.core.isDesktopMode())
                    {

                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);

                            if (Path.GetExtension(file).ToLower() != ".html")
                            {
                                string destFile = Path.Combine(destinationPath, fileName);
                                File.Move(file, destFile, true); // ή Copy αν θες αντίγραφα
                            }
                        }
                    }
                    document.FullPath = destinationFile;
                    document.ParentDirectory = destinationPath;
                    document.Name = Path.GetFileName(destinationFile);

                }
#endif
                   // else
                    {
#if ANDROID
                        FileCopier.CopyFolderToDownloads(document.ParentDirectory,
                          Path.GetFileNameWithoutExtension(savePath));
#endif
                   // }

                    
                }
            }

            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                 
            }
        }
        public void CloseDocument(DocumentInfo document)
        {
            try
            {
                if (Directory.Exists(document.ParentDirectory))
                {
                    Directory.Delete(Path.Combine(document.ParentDirectory, "output.html"), true);
                }
                document = null;
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                 
            }
        }
    }
}
