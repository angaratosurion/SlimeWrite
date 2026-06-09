using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using SlimeMarkUp.Core;
using SlimeWrite.Core;
using SlimeWrite.Core.Models;

#if ANDROID
using SlimeWrite.Platforms.Android;
#endif
using SlimeWrite.Views;
using System.Text;

using AppInfo = SlimeWrite.Core.Models.AppInfo;
using Options = SlimeWrite.Core.Models.Options;
using SlimeWrite.Core.SDK;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Archive;
using SlimeWrite.Core.IO;

namespace SlimeWrite
{
    public  partial class MainPage : ContentPage
    {
        

        public static MarkupParser _parser;
        private readonly HtmlRenderer _renderer;
        private readonly ISaveFileDialog _saveFileDialog;
        AppInfo appInfo;
        DocumentManager documentManager = new DocumentManager();
        DocumentInfo documentInfo;

        public static Kernel core = StaticVariables.core;
        PickOptions PickfileOpenptions = new PickOptions
        {
            PickerTitle = "Open Markdown or SlimeMarkup file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, 
                IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".md", ".markdown", ".zsmd" } },
                { DevicePlatform.Android, new[] { "text/markdown", "application/octet-stream" } },
                { DevicePlatform.iOS, new[] { "public.markdown", "com.pkware.zip-archive" } },
                { DevicePlatform.MacCatalyst, new[] { "public.markdown", "com.pkware.zip-archive" } }
            })

        };

        

        double startHeight, startWidth;

        static Options options = new Options();

        public Editor Editor => editor;
        public WebView Preview => preview;

        public PickOptions PickfileOpenptions1 { get => PickfileOpenptions; set => PickfileOpenptions = value; }

        public MainPage(ISaveFileDialog saveFileDialog)
        {
            try
            {
                _saveFileDialog = saveFileDialog;

                InitializeComponent();
                appInfo = core.GetAppInfo();
                options = core.GetOptions();


              
                PluginManager.LoadPlugins(core.GetPluginsPath());
 

                _parser = core.InitializeParser();
                _renderer = new HtmlRenderer();

                ChangeWindowsTitle(null);

                if (options.AutoUpdateUsingGithub)
                {
                    var updater = new Updater();
                    updater.DownloadLatestRelease();
                }

#if WINDOWS
                var userDataFolder = core.GetAppdataPath();
                Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", userDataFolder);
#endif
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void editor_TextChanged(object? sender, TextChangedEventArgs e)
        {
            try
            {
                // Απενεργοποιούμε το event για να αποφύγουμε infinite loops
                editor.TextChanged -= editor_TextChanged;

                if (editor.Text != null && editor.Text.Contains("\r"))
                {
                    editor.Text = editor.Text.Replace("\r", "\n");
                }

                if (e.OldTextValue != null && e.NewTextValue != null)
                {
                    if (e.NewTextValue.EndsWith("\n") && 
                        options.UpdateOnLosingFocus)
                    {
                        Updatepreview(editor.Text);
                    }
                    else if (options.UseTextChangedEvent)
                    {
                        Updatepreview(editor.Text);
                    }
                }

     // Hook για το event OnEditorTextChanged
                foreach (var plugin in PluginManager.Plugins)
                {
                    plugin.OnEditorTextChanged(editor, e.OldTextValue ?? "", editor.Text ?? "", options);
                }
 
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
            finally
            {
                // Επαναφορά του event listener ανεξάρτητα από τα options για να συνεχίσει να ακούει
                editor.TextChanged += editor_TextChanged;
            }
        }

        private void ChangeWindowsTitle(string filename)
        {
            try
            {
                var AppName = appInfo?.AppName ?? "SlimeWrite";
                var Version = appInfo?.Version ?? "1.0.0";
                var wintile = AppName + " " + Version;
                if (!string.IsNullOrEmpty(filename))
                {
                    wintile += " - " + filename;
                }

                // Διόρθωση: Ανάθεση του τίτλου στο Window της εφαρμογής
                if (Window != null)
                {
                    Window.Title = wintile;
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        async void initilizeOriantation()
        {
            try
            {
                if (options == null || MainGrid == null) return;

                switch (options.WebViewOrientation)
                {
                    case 0:
                        {
                            MainGrid.RowDefinitions.Clear();
                            if (MainGrid.ColumnDefinitions.Count == 0)
                            {
                                MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                                MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                                MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                            }

                            if (MainGrid.ColumnDefinitions.Count > 1)
                            {
                                MainGrid.ColumnDefinitions[1].Width = new GridLength(10);
                            }

                            // Προστασία από κρασάρισμα αν τα controls λείπουν από το XAML
                            if (scrollview != null) MainGrid.SetColumn(scrollview, 0);
                            if (editor != null) MainGrid.SetColumn(editor, 0);
                            if (preview != null) MainGrid.SetColumn(preview, 2);
                            if (this.spliter != null) MainGrid.
                                    SetColumn(this.spliter, 1);
                            break;
                        }
                    case 1:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        public async void OpenFile(string filename)
        {
            try
            {
                FileResult res = null;

                if (filename == null)
                {

                    res = await FilePicker.Default.
                        PickAsync(PickfileOpenptions1);

                    if (res != null)
                    {
                        filename = res.FullPath;
                        if (Path.GetExtension(filename).ToLower() == ".zsmd")
                        {
                            Slime7z.Extract(filename,
                                Path.Combine(core.GetTempfolderPath(),
                                Path.GetFileNameWithoutExtension(filename)));
                            var files = Directory.GetFiles(Path.Combine(
                                core.GetTempfolderPath(),
                                Path.GetFileNameWithoutExtension(filename)));
                            foreach (var file in files)
                            {
                                bool plain = FileHelper.IsPlainTextOnly(file);
                                if (plain)
                                {
                                    filename = file;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            return; // Ο χρήστης ακύρωσε το pick
                        }
                    }

                    // Διόρθωση: Ξεμπλοκάρισμα του event με σωστό τρόπο (-=)
                    editor.TextChanged -= editor_TextChanged;
                    string filecont = "";

                    if (options.SegmentedLoading)
                    {
                        // Χρήση Task αντί για raw Thread για καλύτερη διαχείριση στο MAUI
                        await Task.Run(async () =>
                        {
                            char[] buffer = new char[options.MaxSegmentLength > 0 ? options.MaxSegmentLength * 1024 : 1024 * 1024];
                            StringBuilder sb = new StringBuilder();

                            using (var reader = new StreamReader(filename,
                                Encoding.UTF8))
                            {
                                int bytesRead;
                                while ((bytesRead = await reader.
                                ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    sb.Append(buffer, 0, bytesRead);
                                    await Task.Delay(50);
                                }
                            }

                            filecont = sb.ToString();

                            // Hook για το event OnFileOpened κατά το Segmented Loading
                            foreach (var plugin in PluginManager.Plugins)
                            {
                                plugin.OnFileOpened(filename, ref filecont);
                            }


                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                editor.Text = filecont;
                                FinalizeFileLoad(filename);
                            });
                        });
                    }
                    else
                    {
                        filecont = core.OpenFile(filename);


                        // Hook για το event OnFileOpened κατά το κανονικό Loading
                        foreach (var plugin in PluginManager.Plugins)
                        {
                            plugin.OnFileOpened(filename, ref filecont);
                        }


                        editor.Text = filecont;
                        FinalizeFileLoad(filename);
                    }
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
                // Επαναφορά σε περίπτωση σφάλματος
                Loadeditor();
            }
        }

        private void FinalizeFileLoad(string filename)
        {
            Loadeditor();
            ChangeWindowsTitle(filename);

            this.documentInfo = new DocumentInfo
            {
                FullPath = filename,
                ParentDirectory = Path.GetDirectoryName(filename),
                Name = Path.GetFileName(filename)
            };

            Updatepreview(editor.Text);
        }

        public async Task SaveFile()
        {
            try
            {
                if (documentInfo == null) return;

                string textToSave = editor.Text ?? "";

 
                // Hook για το event OnFileSaving πριν την εγγραφή στο αρχείο
                foreach (var plugin in PluginManager.Plugins)
                {
                    plugin.OnFileSaving(documentInfo.FullPath, 
                        ref textToSave);
                }

                // Χρήση using για αυτόματη αποδέσμευση (Dispose) των Streams (αποφυγή memory leaks)
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter streamWriter = new
                        StreamWriter(stream, Encoding.UTF8, 
                        leaveOpen: true))
                    {
                        await streamWriter.WriteAsync(textToSave);
                        await streamWriter.FlushAsync();
                    }

                    stream.Position = 0;
                      // if (core.isDesktopMode() == false)
                    {

                        //var res = await FileSaver.
                        //    Default.SaveAsync(documentInfo.FullPath, stream);

                        //if (res != null && res.IsSuccessful)
                        //{
                        var path = await _saveFileDialog.
                            PickSaveFileAsync("test.txt", 
                            new[] { ".txt" ,".md",".smd",".zsmd"});

                        if (path is not null)
                        {
                            documentManager.SaveDocument(documentInfo,
                                path, stream);
                            ChangeWindowsTitle(documentInfo.FullPath);
                        }
                    }
                    //else
                    //{
                        
                    //    documentManager.SaveDocument(documentInfo,
                    //        documentInfo.FullPath, stream);

                    //    ChangeWindowsTitle(documentInfo.FullPath);
                    //}
                }
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
                editor.Text = "";
                var popup = new CreateNewDocumentPopUp();

                if (Application.Current?.MainPage != null)
                {
                    IPopupResult<string> result = (IPopupResult<string>)await Application.Current.MainPage.ShowPopupAsync(popup);

                    if (result != null && result.Result != null)
                    {
                        string folderName = result.Result;
                        documentInfo = documentManager.CreateNewDocument(folderName);
                        ChangeWindowsTitle(documentInfo.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void Open_Clicked(object sender, EventArgs e)
        {
            OpenFile(null);
        }

        private void AppOptions_Click(object sender, EventArgs e)
        {
            try
            {
                OptionsView optionsView = new OptionsView();
                if (core.isDesktopMode())
                {
                    var win = new Window(optionsView)
                    {
                        IsMaximizable = false,
                        IsMinimizable = false,
                        Height = optionsView.HeightRequest,
                        Width = optionsView.WidthRequest
                    };
                    Application.Current?.OpenWindow(win);
                }
                else
                {
                    this.Navigation.PushAsync(optionsView);
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
                if (documentInfo != null)
                {
                    documentManager.CloseDocument(documentInfo);
                    documentInfo = null;
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            _ = SaveFile(); // Fire and forget με ασφάλεια
        }

        private void H1_Clicked(object sender, EventArgs e) { AppendText(core.H1_Marked(null)); }
        private void H2_Clicked(object sender, EventArgs e) { AppendText(core.H2_Marked(null)); }
        private void Bold_Clicked(object sender, EventArgs e) { AppendText(core.Bold_Marked(null)); }
        private void Italic_Clicked(object sender, EventArgs e) { AppendText(core.Italic_Marked(null)); }
        private void Link_Clicked(object sender, EventArgs e) { AppendText(core.Link_Marked(null)); }
        private void List_Clicked(object sender, EventArgs e) { AppendText(core.List_Marked(null)); }
        private void Table_Clicked(object sender, EventArgs e) { AppendText(core.Table_Marked(null)); }
        private void Quote_Clicked(object sender, EventArgs e) { AppendText(core.Quote_Marked(null)); }

        private void AppendText(string text)
        {
            try
            {
                editor.Text += text;
            }
            catch (Exception ex) { MainPage.core.ErrorLog(ex); }
        }

        private async void Image_Clicked(object sender, EventArgs e)
        {
            try
            {
                PickOptions fileoptions = new PickOptions { PickerTitle = "Select An Image File" };
                var res = await FilePicker.Default.PickAsync(fileoptions);

                if (res != null)
                {
                    editor.Text += core.Image_Marked(null, res.FullPath, documentInfo);
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            try
            {
                About aboutwindows = new About();
                if (core.isDesktopMode())
                {
                    var win = new Window(aboutwindows)
                    {
                        IsMaximizable = false,
                        IsMinimizable = false,
                        Height = aboutwindows.HeightRequest,
                        Width = aboutwindows.WidthRequest
                    };
                    Application.Current?.OpenWindow(win);
                }
                else
                {
                    this.Navigation.PushAsync(aboutwindows);
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void InserFilepropps_Click(object sender, EventArgs e)
        {
            string props = "---\nfilename: \" \" \ntitle:  \" \"\nauthor:  \" \"\n" +
                           "subject: \" \" \nDescription:\" \" \nPublished: dd/mm/yyyy hh:mm:ss\n" +
                           "keywords: \" \" \ncomments: \" \" \ncompany: \" \" \ncategory: \" \"\n" +
                           "revisionnumber: \" \" \nlanguage:  \" \"\ncontributors:\n  - \"contributor1\"\n" +
                           "  - \"contributor2\"\nVersionHistory: \"1.0.0\"\n---\n";
            AppendText(props);
        }

        private void Loadeditor()
        {
            try
            {
                editor.TextChanged -= editor_TextChanged;
                editor.TextChanged += editor_TextChanged;
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void editor_KeyDown(object? sender, TextChangedEventArgs e)
        {
            Updatepreview(editor.Text);
        }

        private void Updatepreview(string markdown)
        {
            try
            {
                if (string.IsNullOrEmpty(markdown)) return;

                var md = _parser.Parse(markdown);
                if (md != null)
                {
                    var html = "<html>\r\n<head>\r\n <meta charset=\"UTF-8\" /></head><style>\r\n" +
                               "body {\r\n color:black; } </style>" +
                               " <body>" +
                               _renderer.Render(md) + "</body>\r\n</html>";

                    NavigateToStringFile(html);
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        void NavigateToStringFile(string html)
        {
            try
            {
                string file = documentInfo != null
                    ? Path.Combine(documentInfo.ParentDirectory, "output.html")
                    : Path.Combine(core.GetTempfolderPath(), "output.html");

                File.WriteAllText(file, html);
                if (preview != null)
                {
                    preview.Source = file;

#if ANDROID
                    preview.Dispatcher.Dispatch(() =>
                    {
                        try
                        {
                            if (preview.Handler?.PlatformView is Android.Webkit.WebView web)
                            {
                                web.Settings.AllowFileAccess = true;
                                web.LoadUrl($"file://{file}");
                            }
                        }
                        catch { }
                    });
#endif
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        void SetGridContentSizes()
        {
            try
            {
                if (options == null || MainGrid == null) return;

                double editorandpreviewheight = this.Height / 2;
                MainGrid.HeightRequest = this.Height;
                MainGrid.WidthRequest = this.Width;

                if (options.WebViewOrientation == 1)
                {
                    if (editor != null && preview != null)
                    {
                        preview.HeightRequest = editorandpreviewheight;
                        editor.HeightRequest = editorandpreviewheight;
                        preview.WidthRequest = this.Width;
                        editor.WidthRequest = this.Width;
                    }
                }
                else
                {
                    if (editor != null && preview != null)
                    {
                        preview.WidthRequest = MainGrid.Width / 2;
                        editor.WidthRequest = MainGrid.Width / 2;
                        preview.HeightRequest = this.Height / 2;
                        editor.HeightRequest = this.Height / 2;
                    }
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {
            SetGridContentSizes();
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
                if (documentInfo != null)
                {
                    documentManager.CloseDocument(documentInfo);
                }
                Application.Current?.Quit();
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            base.OnAppearing();
            Loadeditor();
            initilizeOriantation();
        }

        private void ImportFile_Clicked(object sender, EventArgs e)
        {
            // Fallback για το παλιό button
            ImportFileAction();
        }

        private async void ImportFileAction()
        {
            try
            {
                PickOptions fileoptions = new PickOptions { PickerTitle = "Select An SlimeMarkdown File" };
                var res = await FilePicker.Default.PickAsync(fileoptions);

                if (res != null)
                {
                    editor.Text += core.IncludeFile_Marked(null, res.FullPath, documentInfo);
                }
            }
            catch (Exception ex) { MainPage.core.ErrorLog(ex); }
        }

        private void editor_Completed(object sender, EventArgs e)
        {
            try
            {
                editor.Text = editor.Text?.Replace("\r", "\n") ?? "";
                if (editor.Text.EndsWith("\n") && options.UpdateOnLosingFocus)
                {
                    Updatepreview(editor.Text);
                }
                else if (options.UseTextChangedEvent)
                {
                    Updatepreview(editor.Text);
                }
                foreach (var plugin in PluginManager.Plugins)
                {
                    plugin.OnEditorCompleted(editor, options);
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            try
            {
                if (options.WebViewOrientation == 1 && TopRow != null && BottomRow != null)
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
                            BottomRow.Height = new GridLength(MainGrid.Height - newHeight - 5, GridUnitType.Absolute);
                            break;
                    }
                    editor.HeightRequest = TopRow.Height.Value;
                    preview.HeightRequest = BottomRow.Height.Value;
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
            }
        }
    }
}