using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SlimeMarkUp.Core;
using SlimeMarkUp.Core.Extensions.SlimeMarkup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue.Avalonia;
using SlimeWrite.Core.Models;
using SlimeWrite.Core;
namespace SlimeWrite.Avalonia.Views;

public partial class MainWindow : Window
{
    string _markdown = "#Καλημέρα";
    private readonly MarkupParser _parser;
    private readonly HtmlRenderer _renderer;
    AppInfo appInfo;
    private AvaloniaCefBrowser Preview;
    Kernel core = new Kernel();


    // CoreWebView2Environment env;
    static Options options = new Options
    {
        //UseTextChangedEvent = true,
        //UseEnterPressed = false
    };
    public MainWindow()
    {
        InitializeComponent();
        initilizePreview();
         
        appInfo = core.GetAppInfo();
        options = core.GetOptions();
        LoadEditor();

        _parser = core.InitializeParser();

        _renderer = new HtmlRenderer();


        Editor.Text = _markdown;
        //UpdatePreview(_markdown);
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
    private void Editor_TextChanged(object? sender, EventArgs e)
    {
        _markdown = Editor.Text;//?? "";

        UpdatePreview(Editor.Text);
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
        this.Title = wintile;
    }


    // ---------------- Toolbar buttons ---------------- //
    private async void OpenFile(string filename)
    {
        //Editor.TextChanged -= Editor_TextChanged;
        if (filename == null)
        {


            var res = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Open Markdown or SlimeMarkup file",
                FileTypeFilter = new List<FilePickerFileType>
                    {
                        new FilePickerFileType("Markdown or SlimeMarkup")
                        {
                            Patterns = new List<string> { "*.*", "*.md", "*.smd" }
                        }
                    }
            });

            if (res != null && res[0]!=null)
            {
                var file = core.OpenFile(res[0].TryGetLocalPath());
                //  Editor.ClearAll();
                Editor.Text = file;


            }
        }
        else
        {
            var file = core.OpenFile(filename);
            Editor.Text = file;
        }
        ChangeWindowsTitle(filename);
        //UpdatePreview(Editor.Text);
        //Editor.TextChanged += Editor_TextChanged;

    }
    private async Task SaveFile()
    {

        FilePickerSaveOptions filePickerSaveOptions = new FilePickerSaveOptions();

        var res = await this.StorageProvider.SaveFilePickerAsync(filePickerSaveOptions);


        if (res != null)
        {
            core.SaveFile(res.TryGetLocalPath(), Editor.Text);
           
            ChangeWindowsTitle(res.Name);


        }

    }
    private void New_Clicked(object sender, RoutedEventArgs e)
    {

        Editor.Text = "";
        ChangeWindowsTitle(null);

    }

    private void Open_Clicked(object sender, RoutedEventArgs e)
    {

        OpenFile(null);

    }
    private void AppOptions_Click(object sender, RoutedEventArgs e)
    {

         OptionsView options = new OptionsView();
        options.Show();
       

    }
    private void Close_Clicked(object sender, RoutedEventArgs e)
    {
        Editor.Text = "";
        ChangeWindowsTitle(null);

    }
    private void Save_Clicked(object sender, RoutedEventArgs e)
    {

        SaveFile();

    }

    private void H1_Clicked(object sender, RoutedEventArgs e)
    {
        if (Editor.SelectedText != null
            && Editor.SelectedText != String.Empty)
        {
            string selectedtext = Editor.SelectedText;
            //Editor.SelectedText.Replace(selectedtext, "#" + selectedtext + "\n");
            Editor.SelectedText = core.H1_Marked(Editor.SelectedText);
        }
        else
        {
            Editor.Text += core.H1_Marked(null);
        }
        
    }

    private void H2_Clicked(object sender, RoutedEventArgs e)
    {
        if (Editor.SelectedText != null
            && Editor.SelectedText != String.Empty)
        {
            string selectedtext = Editor.SelectedText;
            Editor.SelectedText= core.H2_Marked(Editor.SelectedText);
        }
        else
        {
            Editor.Text += core.H2_Marked(null);
        }
    }

    private void Bold_Clicked(object sender, RoutedEventArgs e)
    {
        if (Editor.SelectedText != null
            && Editor.SelectedText != String.Empty)
        {
            string selectedtext = Editor.SelectedText;
            Editor.SelectedText=core.Bold_Marked(Editor.SelectedText);
        }
        else
        {
            Editor.Text += core.Bold_Marked(null);
        }
    }

    private void Italic_Clicked(object sender, RoutedEventArgs e)
    {
        if (Editor.SelectedText != null
            && Editor.SelectedText != String.Empty)
        {
            string selectedtext = Editor.SelectedText;
            Editor.SelectedText=core.Italic_Marked(Editor.SelectedText);
        }
        else
        {
            Editor.Text += core.Italic_Marked(null);
        }
    }

    private void Link_Clicked(object sender, RoutedEventArgs e)
    {
        if (Editor.SelectedText != null
            && Editor.SelectedText != String.Empty)
        {
            string selectedtext = Editor.SelectedText;
            Editor.SelectedText = core.Link_Marked(selectedtext);
        }
        else
        {
            Editor.Text += core.Link_Marked(null);
        }
    }

    private void Image_Clicked(object sender, RoutedEventArgs e)
    {
        if (Editor.SelectedText != null
            && Editor.SelectedText != String.Empty)
        {
            string selectedtext = Editor.SelectedText;
            Editor.SelectedText=core.Image_Marked(selectedtext);
        }
        else
        {
            Editor.Text += core.Image_Marked(null);
        }
    }

    private void List_Clicked(object sender, RoutedEventArgs e)
    {
        Editor.Text += core.List_Marked(null);
    }

    private void Table_Clicked(object sender, RoutedEventArgs e)
    {
        Editor.Text +=core.Table_Marked(null);
    }

    private void Quote_Clicked(object sender, RoutedEventArgs e)
    {
        if (Editor.SelectedText != null
            && Editor.SelectedText != String.Empty)
        {
            string selectedtext = Editor.SelectedText;
            Editor.SelectedText=core.Quote_Marked(selectedtext);
        }
        else
        {
            Editor.Text +=core.Quote_Marked(null);
        }
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        About aboutwindows = new About();
        aboutwindows.Show();
    }

    private void InserFilepropps_Click(object sender, RoutedEventArgs e)
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
        Editor.Text += props + "\n";
    }
    private void LoadEditor()
    {




        // Syntax highlighting for C#
        //Editor.ConfigurationManager.Language = "cs";\
        //Editor.Font = new System.Drawing.Font("Consolas", 16);

        // Faster for large files

       

        if (options.UseTextChangedEvent)
        {
            Editor.TextChanged += Editor_TextChanged;
        }
        if (options.UseEnterPressed)
        {
            Editor.KeyDown += Editor_KeyDown;
        }
    }



    private void Editor_KeyDown(object? sender, RoutedEventArgs e)
    {
        //if (e == Key.Enter)
        {
            _markdown = Editor.Text;//?? "";
            UpdatePreview(Editor.Text);
        }
    }

    private async void UpdatePreview(string markdown)
    {
        var md = _parser.Parse(markdown);
        if (md != null && markdown != "")
        {


            var html = "<html>\r\n<head>\r\n <meta charset=\"UTF-8\" /></head><style>\r\n                        " +
                "body {\r\n    color:black; }  </style>" +
                " <body>" +
                _renderer.Render(md) + "</body>\r\n</html>";



            // await Preview.EnsureCoreWebView2Async(env);

            //  Preview.NavigateToString(html);
            NavigateToStringFile(html);

        }





    }
    async void initilizePreview()
    {
        // env = await CoreWebView2Environment.
        //CreateAsync(userDataFolder:
        //Path.Combine(Path.GetTempPath(), "SlimeWrite"));

        // NOTE: this waits until the first page is navigated - then continues
        //       executing the next line of code!

            Preview=  new AvaloniaCefBrowser();
        BrowserView.Child = Preview;
        //await Preview.EnsureCoreWebView2Async(env);
        switch (options.WebViewOrientation)
        {
            case 0:
                {

                    break;
                }
            case 1:
                {
                    grid.ColumnDefinitions.Clear();
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    this.grid.RowDefinitions[1].Height = new GridLength(5);
                    Grid.SetRow(this.BrowserView, 0);
                    Grid.SetRow(Preview, 2);
                   Grid.SetRow(splinter, 1);
                    break;
                }
        }

    }

    async void NavigateToStringFile(string html)
    {
        string appname = appInfo.AppName;

        var file = Path.Combine(Path.GetTempPath(),appname,
            "output.html");
         if ( Directory.Exists(Path.Combine(Path.GetTempPath(), appname)) == false)
        {
            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), appname));
        }
        File.WriteAllText(file, html);
        if (Preview != null)
        {
            Preview.Address = file;
        }



    }
}