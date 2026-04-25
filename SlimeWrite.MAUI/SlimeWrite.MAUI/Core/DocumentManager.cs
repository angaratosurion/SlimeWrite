using SlimeWrite.MAUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SlimeWrite.MAUI.Core
{
    public class DocumentManager
    {
        Kernel core = new Kernel();
        public DocumentInfo CreateNewDocument(string name)
        {
            DocumentInfo ap = new DocumentInfo();
            if (Path.HasExtension(name))
            {
                string ext = Path.GetExtension(name);
                ap.Name = name;
                string path = Path.Combine(core.GetTempfolderPath(),
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
                string path = Path.Combine(core.GetTempfolderPath(),
                name);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
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
    }
}
