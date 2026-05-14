//using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;
using SlimeMarkUp.Core;
using SlimeWrite.Core;
using SlimeWrite.Core.Models;
#if ANDROID
using SlimeWrite.Platforms.Android;
#endif
using SlimeWrite.Views;
using System.Text;

//using static System.Net.Mime.MediaTypeNames;
using AppInfo = SlimeWrite.Core.Models.AppInfo;
using Options = SlimeWrite.Core.Models.Options;

namespace SlimeWrite
{
    public partial class MainPage : ContentPage
    {
        // string _markdown = "#Καλημέρα";
       
        AppInfo appInfo;
        DocumentManager documentManager = new DocumentManager();
        
        public static DocumentInfo ActivedocumentInfo;

        public static Kernel core = new Kernel();
        //public MarkupParser Parser { get { return _parser; } }
        //public HtmlRenderer Renderer { get { return _renderer; } }

        Editor editor;
        WebView preview;
      public   static Options options = new Options
        {
            //UseTextChangedEvent = true,
            //UseEnterPressed = false
        };
        public Editor Editor {
            get { return editor; }
        }
        public WebView Preview { get { return preview; } }
        public MainPage()
        {
            try
            {
                InitializeComponent();
                appInfo = core.GetAppInfo();
                options = core.GetOptions();

                //   Loadeditor();




               

                //editor.Text = _markdown;
                //Updatepreview(_markdown);
                ChangeWindowsTitle(null);

                //string[] args = Environment.GetCommandLineArgs();
                //if (args.Length > 1)
                //{
                //    OpenFile(args[1]);
                //}
                if (options.AutoUpdateUsingGithub)
                {

                    var updater = new Updater();
                    updater.DownloadLatestRelease();

                }
                //  initilizeOriantation();

                //  this.SetGridContentSizes();

#if WINDOWS
var userDataFolder = core.GetAppdataPath();
Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", userDataFolder);
#endif
                 editor =((MainPage) MainGrid.SelectedItem).Editor;
                preview = ((MainPage)MainGrid.SelectedItem).Preview;
                  

            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }


        }
       

        private void ChangeWindowsTitle(string filename)
        {
            try
            {
                var AppName = appInfo.AppName;
                var Version = appInfo.Version;
                var wintile = AppName + " " + Version;
                if (filename != null)
                {
                    wintile += " - " + filename;
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }
       

        // ---------------- Toolbar buttons ---------------- //
        public async void OpenFile(string filename)
        {
            try
            {
                Stream stream = null;

                //editor.TextChanged -= editor_TextChanged;
                if (filename == null)
                {
                    //#if ANDROID
                    //                var picker = MauiApplication.Current.Services.GetService<IFilePickerService>();

                    //                var result = await picker.PickFileAsync();

                    //                if (result.stream != null)
                    //                {
                    //                    filename = result.name;
                    //                    stream= result.stream;

                    //                }

                    //#endif
                    //#if WINDOWS
                    PickOptions fileoptions = new PickOptions
                    {
                        PickerTitle = "Open Markdown or SlimeMarkup file",

                    };
                    var res = await FilePicker.Default.PickAsync(fileoptions);


                    if (res != null)
                    {


                        filename = res.FullPath;


                    }
                    //#endif

                    editor.TextChanged += null;
                    if (options.SegmentedLoading)
                    {

                        Thread thread = new Thread(() =>
                        {
                            // core.OpenSegmentedFile(filename, ref editor)
                            ;
                            //string filecont = core.OpenFile(filename);
                            string filecont = null;
                            char[] buffer; // 1MB buffer 
                            if (options.MaxSegmentLength > 0)
                            {
                                buffer = new char[options.MaxSegmentLength * 1024];
                            }
                            else
                            {
                                buffer = new char[1024 * 1024];
                            }
                            var reader = new StreamReader(res.FullPath, Encoding.UTF8);
                            int bytesRead;
                            //ReadAllText(filename, Encoding.UTF8);//
                            //   editor.Text = ""; // Καθαρίζει το Text πριν ξεκινήσει η ανάγνωση
                            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                string chunk = new string(buffer, 0, bytesRead);
                                filecont += chunk;
                                Task.Delay(50); // Καθυστερεί λίγο την ανανέωση του UI
                            }
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                // string file = null;
                                editor.Text = filecont;

                            });

                        });
                        thread.Start();

                    }
                    else
                    {
                        var file = core.OpenFile(filename);
                        editor.Text = file;
                    }
                     


                    ChangeWindowsTitle(filename);
                     ActivedocumentInfo = new DocumentInfo();
                    ActivedocumentInfo.FullPath = filename;
                    ActivedocumentInfo.ParentDirectory = Path.GetDirectoryName(filename);
                    ActivedocumentInfo.Name = Path.GetFileName(filename);
                    //Updatepreview(editor.Text);
                    //editor.TextChanged += editor_TextChanged;

                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }
        private async Task SaveFile()
        {

            try
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter streamWriter = new StreamWriter(stream);
                streamWriter.Write(editor.Text);
                streamWriter.Flush();




                //#if WINDOWS
                PickOptions pickOptions = new PickOptions();
                pickOptions.FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, 
                    IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".md", ".markdown", ".smark" } },
                { DevicePlatform.Android, new[] { "text/markdown", "text/plain" } },
                { DevicePlatform.iOS, new[] { "public.plain-text", "net.daringfireball.markdown" } },
                { DevicePlatform.MacCatalyst, new[] { "public.plain-text", "net.daringfireball.markdown" } }
            });
                var res = await FileSaver.Default.SaveAsync(ActivedocumentInfo.FullPath, stream);
                // var res = await FilePicker.Default.PickAsync(pickOptions);
                if (res != null)
                {
                    // core.SaveFile(res.FileName, editor.Text);
                    documentManager.SaveDocument(ActivedocumentInfo, res.FilePath, stream);
                    //documentInfo.FullPath = res.FullPath;
                    //documentInfo.ParentDirectory = Path.GetDirectoryName(res.FullPath);
                    //documentInfo.Name = Path.GetFileName(res.FullPath);

                    ChangeWindowsTitle(res.FilePath);
                    var doc = ActivedocumentInfo;

                }
                streamWriter.Close();
                streamWriter.Dispose();

                stream.Close();
                stream.Dispose();

                //#endif
                //#if ANDROID

                //            var service = MauiApplication.Current.Services.GetService<FileSaveService>();

                //            await service.SaveAsync(stream, "myfile.txt");


                //#endif

            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }


        }
        private async void New_Clicked(object sender, EventArgs e)
        {
            try
            {
                MainGrid.Children.Add(new MainPage());
                editor.Text = "";
                var popup = new CreateNewDocumentPopUp();



                IPopupResult<string> result = (IPopupResult<string>)await Application.Current.MainPage.ShowPopupAsync(popup);

                if (result != null)
                {
                    string folderName = result.Result;



                    ActivedocumentInfo = documentManager.CreateNewDocument(folderName);
                    ChangeWindowsTitle(ActivedocumentInfo.Name);

                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }

        private void Open_Clicked(object sender, EventArgs e)
        {

            try
            {
                OpenFile(null);
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }


        }
        private void AppOptions_Click(object sender, EventArgs e)
        {
            try
            {
                OptionsView options = new OptionsView();
                var win = new Window(options);
                win.IsMaximizable = false;
                win.IsMinimizable = false;

                win.Height = options.HeightRequest;
                win.Width = options.WidthRequest;

                if (core.isDesktopMode())
                {

                    Microsoft.Maui.Controls.Application.Current.OpenWindow(win);

                }
                else
                {
                    OptionsView optionsMobile = new OptionsView();
                    this.Navigation.PushAsync(optionsMobile);
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }


        }
        private void Close_Clicked(object sender, EventArgs e)
        {
            try
            {

                editor.Text = "";
                ChangeWindowsTitle(null);
                documentManager.CloseDocument(ActivedocumentInfo);
                ActivedocumentInfo = null;
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }
        private void Save_Clicked(object sender, EventArgs e)
        {
            try
            {
                SaveFile();
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }

        private void H1_Clicked(object sender, EventArgs e)
        {

            try
            {
                //if (editor.SelectedText != null
                //    && editor.SelectedText != String.Empty)
                //{
                //    string selectedtext = editor.SelectedText;
                //    //editor.SelectedText.Replace(selectedtext, "#" + selectedtext + "\n");
                //    editor.SelectedText = core.H1_Marked(editor.SelectedText);
                //}
                //else
                //{
                editor.Text += core.H1_Marked(null);
                //}
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }

        private void H2_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (editor.SelectedText != null
                //    && editor.SelectedText != String.Empty)
                //{
                //    string selectedtext = editor.SelectedText;
                //    editor.SelectedText = core.H2_Marked(editor.SelectedText);
                //}
                //else
                //{
                editor.Text += core.H2_Marked(null);
                //  }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }

        private void Bold_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (editor.SelectedText != null
                //    && editor.SelectedText != String.Empty)
                //{
                //    string selectedtext = editor.SelectedText;
                //    editor.SelectedText = core.Bold_Marked(editor.SelectedText);
                //}
                //else
                //{
                editor.Text += core.Bold_Marked(null);
                // }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }

        private void Italic_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (editor.SelectedText != null
                //    && editor.SelectedText != String.Empty)
                //{
                //    string selectedtext = editor.SelectedText;
                //    editor.SelectedText = core.Italic_Marked(editor.SelectedText);
                //}
                //else
                //{
                editor.Text += core.Italic_Marked(null);
                //} 
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }

        private void Link_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (editor.SelectedText != null
                //    && editor.SelectedText != String.Empty)
                //{
                //    string selectedtext = editor.SelectedText;
                //    editor.SelectedText = core.Link_Marked(selectedtext);
                //}
                //else
                //{
                editor.Text += core.Link_Marked(null);
                //  }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }

        private async void Image_Clicked(object sender, EventArgs e)
        {
            try
            {
                string imagename = "";
                PickOptions fileoptions = new PickOptions
                {
                    PickerTitle = "Select An Image File",

                };
                var res = await FilePicker.Default.PickAsync(fileoptions);


                if (res != null)
                {


                    imagename = res.FullPath;


                }
                editor.Text += core.Image_Marked(null, imagename, ActivedocumentInfo);
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
            //}
        }

        private void List_Clicked(object sender, EventArgs e)
        {
            try
            {
                editor.Text += core.List_Marked(null);
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }

        private void Table_Clicked(object sender, EventArgs e)
        {
            try
            {
                editor.Text += core.Table_Marked(null);
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }

        private void Quote_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (editor.SelectedText != null
                //    && editor.SelectedText != String.Empty)
                //{
                //    string selectedtext = editor.SelectedText;
                //    editor.SelectedText = core.Quote_Marked(selectedtext);
                //}
                //else
                //{
                editor.Text += core.Quote_Marked(null);
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
            //}
        }

        private void About_Click(object sender, EventArgs e)
        {
            try
            {
                About aboutwindows = new About();
                var win = new Window(aboutwindows);
                win.IsMaximizable = false;
                win.IsMinimizable = false;
                win.Height = aboutwindows.HeightRequest;
                win.Width = aboutwindows.WidthRequest;
                if (core.isDesktopMode())
                {

                    Application.Current.OpenWindow(win);
                }
                else
                {
                    About aboutMobile = new About();
                    this.Navigation.PushAsync(aboutMobile);


                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }

        private void InserFilepropps_Click(object sender, EventArgs e)
        {
            try
            {
                const string props = "---\r\n  filename: \" \" \r\n  " +
                    "title:  \" \"\r\n  author:  \" \"\r\n  " +
                    "subject: \" \" \r\n  Description:\" \" \r\n  " +
                    "Published: dd/mm/yyyy hh:mm:ss\r\n  " +
                    "keywords: \" \" \r\n  comments: \" \" \r\n  " +
                    "company: \" \" \r\n  category: \" \"\r\n  " +
                    "revisionnumber: \" \" \r\n  language:  \" \"\r\n  " +
                    "contributors:\r\n    - \"contributor1\"\r\n    " +
                    "- \"contributor2\"\r\n  VersionHistory: \"1.0.0\"\r\n---";
                editor.Text += props + "\n";
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }
     
       

        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                this.EditorPreview.SetGridContentSizes();
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    OpenFile(args[1]);
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);



            }
        }

        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            try
            {
                core.ClearTempFolder();
                Microsoft.Maui.Controls.Application.Current.Quit();
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }

        private void MainGrid_CurrentPageChanged(object sender, EventArgs e)
        {
            try
            {
                ActivedocumentInfo = (EditorPreview).DocumentInfo;
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }
    }
}

