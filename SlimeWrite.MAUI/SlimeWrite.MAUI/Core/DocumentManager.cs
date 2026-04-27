using SlimeWrite.MAUI.Core.Models;

namespace SlimeWrite.MAUI.Core
{
    public class DocumentManager
    {
        
        public DocumentInfo CreateNewDocument(string name)
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
        public void AddFileToDocumentParentDirectory(DocumentInfo document, string filePath)
        {

            string fileName = Path.GetFileName(filePath);
            string destinationPath = Path.Combine(document.ParentDirectory, fileName);
            File.Copy(filePath, destinationPath, true);

        }
        public void SaveDocument(DocumentInfo document, string savePath,Stream stream)
        {
            if (Directory.Exists(document.ParentDirectory) && stream != null)
            {
                string destinationPath = Path.Combine(Path.GetDirectoryName(savePath),
                        Path.GetFileNameWithoutExtension(savePath));
                Directory.CreateDirectory(destinationPath);

                foreach (string file in Directory.GetFiles(document.ParentDirectory))
                {
                    string fileName = Path.GetFileName(file);
                    
                    if (!Path.HasExtension(".html"))
                    {
                        File.Copy(file, destinationPath, true);
                    }
                    
                    
                }
                StreamReader streamReader = new StreamReader(stream);
                File.WriteAllTextAsync(document.FullPath, streamReader.ReadToEnd());
                stream.Close();
                streamReader.Close();
                document.FullPath = savePath;
                document.ParentDirectory = destinationPath;
                document.Name = Path.GetFileName(savePath);

            }
        }
    }
}
