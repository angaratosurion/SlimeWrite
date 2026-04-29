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
                string destinationPath = Path.Combine(
                    Path.GetDirectoryName(savePath),
                    Path.GetFileNameWithoutExtension(savePath));

                Directory.CreateDirectory(destinationPath);

                string destinationFile = Path.Combine(destinationPath, Path.GetFileName(savePath));
                File.Move(savePath, destinationFile, true);

                var files = Directory.GetFiles(document.ParentDirectory);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);

                    if (Path.GetExtension(file).ToLower() != ".html")
                    {
                        string destFile = Path.Combine(destinationPath, fileName);
                        File.Move(file, destFile, true); // ή Copy αν θες αντίγραφα
                    }
                }

                document.FullPath = destinationFile;
                document.ParentDirectory = destinationPath;
                document.Name = Path.GetFileName(destinationFile);
            }
        }
        public void CloseDocument(DocumentInfo document)
        {
            if (Directory.Exists(document.ParentDirectory))
            {
                Directory.Delete(Path.Combine(document.ParentDirectory, "output.html"), true);
            }
            document = null;
        }
    }
}
