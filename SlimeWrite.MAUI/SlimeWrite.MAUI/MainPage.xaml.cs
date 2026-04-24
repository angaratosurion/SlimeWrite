//using CommunityToolkit.Maui.Storage;
using SlimeMarkUp.Core;
using SlimeWrite.MAUI.Core;
using SlimeWrite.MAUI.Core.Models;
#if ANDROID
using SlimeWrite.MAUI.Platforms.Android;
#endif
using SlimeWrite.MAUI.Views;
using System.Text;

//using static System.Net.Mime.MediaTypeNames;
using AppInfo = SlimeWrite.MAUI.Core.Models.AppInfo;

namespace SlimeWrite.MAUI
{
    public partial class MainPage : ContentPage
    {
       // string _markdown = "#Καλημέρα";
        private readonly MarkupParser _parser;
        private readonly HtmlRenderer _renderer;
        AppInfo appInfo;
       

      public static   Kernel core = new Kernel();
        double startHeight ,startWidth;
        
        static Options options = new Options
        {
            //UseTextChangedEvent = true,
            //UseEnterPressed = false
        };
        public Editor Editor  {
            get { return editor; }
        }
        public WebView Preview { get { return preview; } }
        public MainPage()
        {
            InitializeComponent();
            appInfo = core.GetAppInfo();
            options = core.GetOptions();
            
         //   Loadeditor();




            _parser = core.InitializeParser();

            _renderer = new HtmlRenderer();


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
var userDataFolder = Path.Combine(FileSystem.AppDataDirectory, core.GetAppInfo().AppName);
Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", userDataFolder);
#endif




        }
        private void editor_TextChanged(object? sender, EventArgs e)
        {
            //_markdown = editor.Text;//?? "";
          //  editor.Text = editor.Text.Replace("\r", "\n");
            Updatepreview(editor.Text);
        }

        private void ChangeWindowsTitle(string filename)
        {

            var AppName = appInfo.AppName;
            var Version = appInfo.Version;
            var wintile = AppName + " " + Version;
            if (filename != null)
            {
                wintile += " - " + filename;
            }

        }
        async void initilizeOriantation()
        {

            //this.MainGrid.HeightRequest = this.Height;

            switch (options.WebViewOrientation)
            {
                
                case 0:
                    {
                        this.MainGrid.RowDefinitions.Clear();
                        this.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                        

                        this.MainGrid.ColumnDefinitions.
                            Add(new ColumnDefinition());
                        this.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                        this.MainGrid.ColumnDefinitions[1].Width = new GridLength(10);
                        this.MainGrid.Children.Clear();

                        MainGrid.SetColumn(scrollview, 0);
                        MainGrid.SetColumn(editor, 0)   ;
                        MainGrid.SetColumn(preview, 2);
                        MainGrid.SetColumn(this.spliter, 1);
                        

                        //this.ContentPage_SizeChanged(null, null);
                       // this.SetGridContentSizes();
                        break;
                      

                    }
                case 1:
                    {
                       
                        break;
                    }
                   
            }
             
        }

        // ---------------- Toolbar buttons ---------------- //
        public   async void OpenFile(string filename)
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
                Loadeditor();


                ChangeWindowsTitle(filename);
                //Updatepreview(editor.Text);
                //editor.TextChanged += editor_TextChanged;

            }
        }
        private async Task SaveFile()
        {
           

            MemoryStream stream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            streamWriter.Write(editor.Text);
            streamWriter.Flush();




#if WINDOWS
             PickOptions pickOptions = new PickOptions();
            pickOptions.FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".md", ".markdown", ".smark" } },
                { DevicePlatform.Android, new[] { "text/markdown", "text/plain" } },
                { DevicePlatform.iOS, new[] { "public.plain-text", "net.daringfireball.markdown" } },
                { DevicePlatform.MacCatalyst, new[] { "public.plain-text", "net.daringfireball.markdown" } }
            });
              var res = await FilePicker.Default.PickAsync(pickOptions);
             if (res != null)
            {
               // core.SaveFile(res.FileName, editor.Text);

                ChangeWindowsTitle(res.FileName);


            }
             streamWriter.Close();
            streamWriter.Dispose();
            
            stream.Close();
            stream.Dispose();
            
#endif
#if ANDROID

            var service = MauiApplication.Current.Services.GetService<FileSaveService>();

            await service.SaveAsync(stream, "myfile.txt");


#endif
           

           

        }
        private void New_Clicked(object sender, EventArgs e)
        {

            editor.Text = "";
            ChangeWindowsTitle(null);

        }

        private void Open_Clicked(object sender, EventArgs e)
        {
 
             
            OpenFile(null);


        }
        private void AppOptions_Click(object sender, EventArgs e)
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
        private void Close_Clicked(object sender, EventArgs e)
        {
            editor.Text = "";
            ChangeWindowsTitle(null);

        }
        private void Save_Clicked(object sender, EventArgs e)
        {

            SaveFile();

        }

        private void H1_Clicked(object sender, EventArgs e)
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

        private void H2_Clicked(object sender, EventArgs e)
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

        private void Bold_Clicked(object sender, EventArgs e)
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

        private void Italic_Clicked(object sender, EventArgs e)
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

        private void Link_Clicked(object sender, EventArgs e)
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

        private void Image_Clicked(object sender, EventArgs e)
        {
            //if (editor.SelectedText != null
            //    && editor.SelectedText != String.Empty)
            //{
            //    string selectedtext = editor.SelectedText;
            //    editor.SelectedText = core.Image_Marked(selectedtext);
            //}
            //else
            //{
            editor.Text += core.Image_Marked(null);
            //}
        }

        private void List_Clicked(object sender, EventArgs e)
        {
            editor.Text += core.List_Marked(null);
        }

        private void Table_Clicked(object sender, EventArgs e)
        {
            editor.Text += core.Table_Marked(null);
        }

        private void Quote_Clicked(object sender, EventArgs e)
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
            //}
        }

        private void About_Click(object sender, EventArgs e)
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

        private void InserFilepropps_Click(object sender, EventArgs e)
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
        private void Loadeditor()
        {




            // Syntax highlighting for C#
            //editor.ConfigurationManager.Language = "cs";\
            //editor.Font = new System.Drawing.Font("Consolas", 16);

            // Faster for large files
            ///this.editor.HeightRequest = this.Height - preview.Height;
            //this.editor.AutoSize = EditorAutoSizeOption.TextChanges;


            if (options.UseTextChangedEvent)
            {
                editor.TextChanged += editor_TextChanged;
            }
            if (options.UseEnterPressed)
            {

                //editor.KeyDown += editor_KeyDown;
            }
        }



        private void editor_KeyDown(object? sender, TextChangedEventArgs e)
        {
            //if (e == Key.Enter)
            {
                //_markdown = editor.Text;//?? "";
                Updatepreview(editor.Text);
            }
        }

        private async void Updatepreview(string markdown)
        {
            var md = _parser.Parse(markdown);
            if (md != null && markdown != "")
            {


                var html = "<html>\r\n<head>\r\n <meta charset=\"UTF-8\" /></head><style>\r\n                        " +
                    "body {\r\n    color:black; }  </style>" +
                    " <body>" +
                    _renderer.Render(md) + "</body>\r\n</html>";



                // await preview.EnsureCoreWebView2Async(env);

                //  preview.NavigateToString(html);
                NavigateToStringFile(html);

            }

            async void NavigateToStringFile(string html)
            {
                try
                { 
                string appname = appInfo.AppName;
                    if (core.isDesktopMode())
                    {

                        var file = Path.Combine(Path.GetTempPath(), appname,
                            "output.html");
                        if (Directory.Exists(Path.Combine(Path.GetTempPath(), appname)) == false)
                        {
                            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), appname));
                        }
                        File.WriteAllText(file, html);
                        if (preview != null)
                        {
                            preview.Source = file;

                        }
                    }
                    else
                    {
                        var file = Path.Combine(FileSystem.CacheDirectory
                            , "output.html");
                        if (Directory.Exists(FileSystem.CacheDirectory) == false)
                        {
                            Directory.CreateDirectory(FileSystem.CacheDirectory);
                        }
                        File.WriteAllText(file, html);
                        if (preview != null)
                        {
                            //HtmlWebViewSource source = new HtmlWebViewSource
                            //{
                            //    Html = html,
                            //    BaseUrl = file
                            //};
                            //preview.Source = source;

                            // WebView.Settings.AllowFileAccess = true;
                            //var source   = new UrlWebViewSource { Url = $"file://{file}" };
                            // preview.Source = source;

#if ANDROID
                            preview.Dispatcher.Dispatch(() =>
                            {

                                try
                                {
                                    Android.Webkit.WebView web = preview.Handler.PlatformView as Android.Webkit.WebView;
                                    web.Settings.AllowFileAccess = true;
                                    web.LoadUrl($"file://{file}");
                                }
                                catch (Exception ex)
                                {
                                }
                            });
#endif


                        }
                    }
                    
                }
                catch (Exception ex)
                {

                }


            }





        }
        void SetGridContentSizes()
        {
            double editorandpreviewheight = this.Height / 2;
            double editorandpreviewwidth = this.Width / 2;
            this.MainGrid.HeightRequest = this.Height;
            this.MainGrid.WidthRequest= this.Width;

            if (options.WebViewOrientation == 1)
            {
                if (editor.Height <= editorandpreviewheight)
                {
                    this.preview.HeightRequest = this.Height / 2;

                    this.editor.HeightRequest = this.Height / 2;

                }
                else
                {
                    this.preview.HeightRequest = this.TopRow.Height.Value;

                    this.editor.HeightRequest = this.BottomRow.Height.Value; ;

                }
                this.preview.WidthRequest = this.Width;
                this.editor.WidthRequest = this.Width; ; ;
            }
            else
            {
                if (editor.Width <= editorandpreviewwidth)
                {
                    this.preview.WidthRequest = this.MainGrid.Width / 2;

                    this.editor.WidthRequest = this.MainGrid.Width / 2;
                }
                else
                {
                    this.preview.WidthRequest = this.MainGrid.Width / 2;

                    this.editor.WidthRequest = this.MainGrid.Width / 2;
                }
                this.preview.HeightRequest = this.Height / 2;
                this.editor.HeightRequest = this.Height / 2;

            }
        }

        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {
            this.SetGridContentSizes();

        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                OpenFile(args[1]);
            }

        }

        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            Microsoft.Maui.Controls.Application.Current.Quit();
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            base.OnAppearing();

            try
            {
                Loadeditor();
                initilizeOriantation();
                string[] args = Environment.GetCommandLineArgs();
                //if (args.Length > 1)
                //{
                //    OpenFile(args[1]);
                //}

                /*  if (editor != null)
                      editor.Text = _markdown;*/
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.ToString(), "OK");
            }

        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (options.WebViewOrientation == 1)
            {
                 
                switch (e.StatusType)
                {
                    case GestureStatus.Started:

                        startHeight = TopRow.Height.Value;
                        break;

                    case GestureStatus.Running:
                        double newHeight = startHeight + e.TotalY;

                        if (newHeight < 50) newHeight = 50;
                        if (newHeight > MainGrid.Height - 50) newHeight = MainGrid.Height - 50;

                        TopRow.Height = new GridLength(newHeight, GridUnitType.Absolute);
                        BottomRow.Height =
                            new GridLength(MainGrid.Height - newHeight - 5, GridUnitType.Absolute);
                        break;
                }
                this.editor.HeightRequest = this.TopRow.Height.Value;
                this.preview.HeightRequest = this.BottomRow.Height.Value;

            }
            else
            {
                
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        startWidth = MainGrid.ColumnDefinitions[0].Width.Value;
                        break;
                    case GestureStatus.Running:
                        double newWidth = startWidth + e.TotalX;
                        if (newWidth < 50) newWidth = 50;
                        if (newWidth > MainGrid.Width - 50) newWidth = MainGrid.Width - 50;
                        MainGrid.ColumnDefinitions[0].Width =
                            new GridLength(newWidth, GridUnitType.Absolute);
                        MainGrid.ColumnDefinitions[2].Width =
                            new GridLength(MainGrid.Width - newWidth - 5, GridUnitType.Absolute);
                        break;
                }
                this.editor.WidthRequest = this.MainGrid.ColumnDefinitions[2].Width.Value;
                this.preview.WidthRequest = this.MainGrid.ColumnDefinitions[0].Width.Value;
            }
            startHeight = 0;
            startWidth = 0;
        }

       
    }
}

