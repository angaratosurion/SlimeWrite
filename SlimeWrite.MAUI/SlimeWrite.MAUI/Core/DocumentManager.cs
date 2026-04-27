using SlimeWrite.MAUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
        public void SaveDocument(DocumentInfo document, string savePath)
        {
            if (Directory.Exists(document.ParentDirectory))
            {
                foreach (string file in Directory.GetFiles(document.ParentDirectory))
                {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(savePath,
                        Path.GetFileNameWithoutExtension(fileName));
                    if (!Path.HasExtension(".html"))
                    {
                        File.Copy(file, destinationPath, true);
                    }
                }

            }
        }
    }
}
