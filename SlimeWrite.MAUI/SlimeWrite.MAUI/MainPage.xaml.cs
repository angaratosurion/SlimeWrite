using SlimeMarkUp.Core;
using SlimeWrite.Core;
using SlimeWrite.Core.Models;
using SlimeWrite.MAUI.Views;
using AppInfo = SlimeWrite.Core.Models.AppInfo;

namespace SlimeWrite.MAUI
{
    public partial class MainPage : ContentPage
    {
        string _markdown = "#Καλημέρα";
        private readonly MarkupParser _parser;
        private readonly HtmlRenderer _renderer;
        AppInfo appInfo;

        Kernel core = new Kernel();
        double startHeight;
        
        static Options options = new Options
        {
            //UseTextChangedEvent = true,
            //UseEnterPressed = false
        };

        public MainPage()
        {
            InitializeComponent();
            appInfo = core.GetAppInfo();
            options = core.GetOptions();
            Loadeditor();




            _parser = core.InitializeParser();

            _renderer = new HtmlRenderer();


            editor.Text = _markdown;
            //Updatepreview(_markdown);
            ChangeWindowsTitle(null);

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                OpenFile(args[1]);
            }
            if (options.AutoUpdateUsingGithub)
            {

                var updater = new Updater();
                updater.DownloadLatestRelease();

            }
           
        }
        private void editor_TextChanged(object? sender, EventArgs e)
        {
            _markdown = editor.Text;//?? "";
            editor.Text = editor.Text.Replace("\r", "\n");
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


        // ---------------- Toolbar buttons ---------------- //
        private async void OpenFile(string filename)
        {
            PickOptions options = new PickOptions
            {
                PickerTitle = "Open Markdown or SlimeMarkup file",
                //FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                //{
                //    { DevicePlatform.WinUI, new[] { "*.*", "*.md", "*.smd" } },
                //    { DevicePlatform.Android, new[] { "*/*", "text/markdown", "application/octet-stream" } },
                //    { DevicePlatform.iOS, new[] { "*/*", "public.plain-text" } },
                //    { DevicePlatform.MacCatalyst, new[] { "*/*", "public.plain-text" } }
                //})
            };

            //editor.TextChanged -= editor_TextChanged;
            if (filename == null)
            {

                var res = await FilePicker.Default.PickAsync(options);
                //var res = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                //{
                //    AllowMultiple = false,
                //    Title = "Open Markdown or SlimeMarkup file",
                //    FileTypeFilter = new List<FilePickerFileType>
                //    {
                //        //new FilePickerFileType("Markdown or SlimeMarkup")
                //        //{
                //        //    Patterns = new List<string> { "*.*", "*.md", "*.smd" }
                //        //}
                //    }
                //});

                if (res != null)
                {
                    var file = core.OpenFile(res.FullPath);
                    //  editor.ClearAll();
                    editor.Text = file;


                }
            }
            else
            {
                var file = core.OpenFile(filename);
                editor.Text = file;
            }
            ChangeWindowsTitle(filename);
            //Updatepreview(editor.Text);
            //editor.TextChanged += editor_TextChanged;

        }
        private async Task SaveFile()
        {
            PickOptions options = new PickOptions
            {
                PickerTitle = "Save Markdown or SlimeMarkup file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { "*.*", "*.md", "*.smd" } },
                    { DevicePlatform.Android, new[] { "*/*", "text/markdown", "application/octet-stream" } },
                    { DevicePlatform.iOS, new[] { "*/*", "public.plain-text" } },
                    { DevicePlatform.MacCatalyst, new[] { "*/*", "public.plain-text" } }
                })
            };



            var res = await FilePicker.Default.PickAsync(options);


            if (res != null)
            {
                core.SaveFile(res.FullPath, editor.Text);

                ChangeWindowsTitle(res.FileName);


            }

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
            Application.Current.OpenWindow(win);

            //options.Show();


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

            Application.Current.OpenWindow(win);

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
            this.editor.HeightRequest = this.Height - preview.Height;
            this.editor.AutoSize = EditorAutoSizeOption.TextChanges;


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
                _markdown = editor.Text;//?? "";
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
                string appname = appInfo.AppName;

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





        }

        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {

            double editorandpreviewheight= this.Height / 2;
            if (editor.Height <= editorandpreviewheight)
            {
                this.preview.HeightRequest = this.Height / 2;
                
                this.editor.HeightRequest = this.Height / 2;
               
            }
            else
            {
                this.preview.HeightRequest = this.TopRow.Height.Value;

                this.editor.HeightRequest = this.BottomRow.Height.Value;
            }
            this.preview.WidthRequest = this.Width;
            this.editor.WidthRequest = this.Width - 25; ;
        }
        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
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
            startHeight = 0;
        }

       
    }
}

