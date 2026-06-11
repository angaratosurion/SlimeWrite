using ImageMagick;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Models;
using SlimeWrite.Core.SDK.Interfaces;

namespace CompressImages
{
    // All the code in this file is included in all platforms.
    public class ImgCompress : ISlimePlugin
    {
        public string Name => "Compress Images";

        public string Version => "1.0.0";

        public string Author => "Your Name";

        public string Description => "Compresses images in the editor";
        public void AddNewButton(FlexLayout toolbar)
        {
            
        }

        public void OnEditorCompleted(Editor editor, Options options)
        {
             
        }

        public void OnEditorTextChanged(Editor editor, string oldText, string newText, Options options)
        {
            
        }

        public void OnFileOpened(string filePath, ref string fileContent)
        {
            
        }

         public void OnFileSaving(string filePath, ref string fileContent)
        {
            var files = Directory.GetFiles(filePath);
            foreach (var file in files)
            {
                if (FileHelper.IsImageFile(file))
                {
                    var image = new MagickImage(file);
                    // Perform compression logic here
                    image.Quality = 75; // Adjust quality as needed
                    image.WriteAsync(file);

                }
            }
        }
    }
}
