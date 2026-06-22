using ImageMagick;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Models;
using SlimeWrite.Core.SDK.Interfaces;

namespace CompressImages
{
    // All the code in this file is included in all platforms.
    public class ImgCompress : ISlimePlugin
    {
         public ImgCompress() {
            var optmnger = new OptionManaer();
            CompressOptions = optmnger.GetOptions();
        }
         CompressOptions CompressOptions { get; set; }
        public string Name => "Compress Images";

        public string Version => "1.0.0";

        public string Author => "Your Name";

        public string Description => "Compresses images in the editor";

        public void AddFileToDocumentParentDirectory(DocumentInfo document, 
            string filePath)
        {
            var files = Directory.GetFiles(filePath);
            foreach (var file in files)
            {
                if (FileHelper.IsImage(file))
                {
                    var image = new MagickImage(file);
                    // Perform compression logic here
                    image.Quality = (uint)this.CompressOptions.Quality; // Adjust quality as needed
                    image.WriteAsync(file);

                }
            }
        }

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
            
        }
        public void OpenPluginSettings(ContentPage MainView)
        {
            try
            {
                ImgCompressOptions imgCompressOptions = new ImgCompressOptions();
                if (StaticVariables.core.isDesktopMode())
                {
                    var win = new Window(imgCompressOptions)
                    {
                        IsMaximizable = false,
                        IsMinimizable = false,
                        Height = imgCompressOptions.HeightRequest,
                        Width = imgCompressOptions.WidthRequest
                    };
                    Application.Current?.OpenWindow(win);
                }
                else
                {
                     
                    MainView.Navigation.PushAsync(imgCompressOptions);
                }


            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);


            }
        }
    }
}
