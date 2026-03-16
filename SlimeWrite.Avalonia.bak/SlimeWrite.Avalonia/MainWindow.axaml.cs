using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SlimeMarkUp.Core;
using SlimeMarkUp.Core.Extensions.SlimeMarkup;
using SlimeWrite.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue.Avalonia;

namespace SlimeWrite.Avalonia
{
    public partial class MainWindow : Window
    {
        string _markdown = "#Καλημέρα";
        private readonly MarkupParser _parser;
        private readonly HtmlRenderer _renderer;
      
        private AvaloniaCefBrowser Preview;


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
            using (StreamReader r = new StreamReader("appsettings.json"))
            {
                string json = r.ReadToEnd();
                options = JsonSerializer.Deserialize<Options>(json);
            }
            LoadEditor();

            _parser = new MarkupParser(new List<IBlockMarkupExtension>
            {
                new HeaderExtension(),
                new ImageExtension(),
                new TableExtension(),
                new ListExtension(),
                new CodeBlockExtension(),
                new BlockquoteExtension(),
                new InlineStyleExtension(),
                new LinkExtension(),
                //new IncludeExtension(), 
                new IncludeCSSExtension()
                 , new IncludeScriptExtension(),
                 new HorizontalRuleExtension(),
                  new EscapeCharsExtension(),
                  new HtmlIgnoreExceptIncludeExtension()

            });

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
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var AppName = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
                "SlimeWrite";
            var Version = asm.GetName()?.Version?.ToString() ?? "1.0.0";
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

                if (res != null)
                {
                    var file = await File.ReadAllTextAsync(res[0].TryGetLocalPath()
                        , Encoding.UTF8
                        , CancellationToken.None);
                  //  Editor.ClearAll();
                    Editor.Text = file;


                }
            }
            else
            {
                var file = await File.ReadAllTextAsync(filename, Encoding.UTF8
                        , CancellationToken.None);
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
                File.WriteAllText(res.TryGetLocalPath(), Editor.Text, Encoding.UTF8);
                ChangeWindowsTitle(res.Name);


            }

        }
        private void New_Clicked(object sender, RoutedEventArgs e)
        {

            Editor.Text= "";
            ChangeWindowsTitle(null);

        }

        private void Open_Clicked(object sender, RoutedEventArgs e)
        {

            OpenFile(null);

        }
        private void Close_Clicked(object sender, RoutedEventArgs e)
        {
            Editor.Text = "";
            ChangeWindowsTitle(null);

        }
        private void Save_Clicked(object sender,  RoutedEventArgs e)
        {

            SaveFile();

        }

        private void H1_Clicked(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedText != null
                && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "#" + selectedtext + "\n");
            }
            else
            {
                Editor.Text += "#Heading 1\n";
            }
        }

        private void H2_Clicked(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedText != null
                && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "\n## " + selectedtext + "\n");
            }
            else
            {
                Editor.Text += "\n## Heading 2\n";
            }
        }

        private void Bold_Clicked(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedText != null
                && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "**" + selectedtext + "**");
            }
            else
            {
                Editor.Text += "**bold**";
            }
        }

        private void Italic_Clicked(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedText != null
                && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "*" + selectedtext + "*");
            }
            else
            {
                Editor.Text += "*italic*";
            }
        }

        private void Link_Clicked(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedText != null
                && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext,
                    "[" + selectedtext + "](url){target=_blank rel=nofollow}");
            }
            else
            {
                Editor.Text += "[text](url){target=_blank rel=nofollow}";
            }
        }

        private void Image_Clicked(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedText != null
                && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "![" + selectedtext +
                    "](image.png){ width = 100 height = 200}");
            }
            else
            {
                Editor.Text += "![alt](image.png){width=100 height=200}";
            }
        }

        private void List_Clicked(object sender, RoutedEventArgs e)
        {
            Editor.Text += "\n- item 1\n- item 2\n";
        }

        private void Table_Clicked(object sender, RoutedEventArgs e)
        {
            Editor.Text += """
        | Col1 | Col2 |
        |------|------|
        | A    | B    |
        | C    | D    |
        """;
        }

        private void Quote_Clicked(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedText != null
                && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "\n>" + selectedtext + "\n");
            }
            else
            {
                Editor.Text += "\n> quote\n";
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
                        //Grid.SetRow(formsHost, 0);
                        //Grid.SetRow(Preview, 2);
                        //Grid.SetRow(splinter, 1);
                        break;
                    }
            }
             
        }

        async void NavigateToStringFile(string html)
        {

            var file = Path.Combine(Path.GetTempPath(),
                "output.html");
            File.WriteAllText(file, html);
            Preview.Address = file;
            


        }
    }
}