using ScintillaNET;
using SlimeMarkUp.Core;
using SlimeMarkUp.Core.Extensions.SlimeMarkup;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace SlimeWrite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _markdown = "#Καλημέρα";
        private readonly MarkupParser _parser;
        private readonly HtmlRenderer _renderer;
        private Scintilla Editor;
        public MainWindow()
        {
            

            InitializeComponent();
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
                new IncludeExtension(), 
                new IncludeCSSExtension()
                 , new IncludeScriptExtension()
            });

            _renderer = new HtmlRenderer();
            //  Preview.EnsureCoreWebView2Async();

            Editor.Text = _markdown;
            //UpdatePreview(_markdown);
             

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length>1)
            {
                OpenFile(args[1]);
            }

        }




        private void LoadEditor()
        {
            Editor = new Scintilla();
            
            // Line numbers
            Editor.Margins[0].Width = 40;

            // Basic styling
            Editor.StyleResetDefault();
            //Editor.Styles[Style.Default].Font = "Consolas";
            //Editor.Styles[Style.Default].Size = 12;
            Editor.StyleClearAll();

            // Syntax highlighting for C#
            //Editor.ConfigurationManager.Language = "cs";\
            //Editor.Font = new System.Drawing.Font("Consolas", 16);
            Editor.Styles[0].Size = 16;

            // Faster for large files
            Editor.ScrollWidthTracking = true;

            // Assign to host
            formsHost.Child = Editor;
            Editor.TextChanged += Editor_TextChanged;
        }

        private void Editor_TextChanged(object? sender, EventArgs e)
        {
            _markdown = Editor.Text;//?? "";
            
               UpdatePreview(_markdown);
        }

        private void UpdatePreview(string markdown)
        {
            var md = _parser.Parse(markdown);
            var html = "<html>\r\n<head>\r\n <meta charset=\"UTF-8\" /></head><style>\r\n                        " +
                "body {\r\n    color:black; }  </style>" +
                " <body>" +
                _renderer.Render(md) + "</body>\r\n</html>";




             
                Preview.NavigateToString(html);
            

            


        }
         

        // ---------------- Toolbar buttons ---------------- //
        private async void OpenFile(string filename)
        {
            if (filename == null)
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = 
                    new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Filter =
                       "All Files (*.*)|*.*|" +
                           "Markdown (*.md)|*.md|" +
                           "SlimeMarkup (*.smd)|*.smd";

                DialogResult res = openFileDialog.ShowDialog();

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    var file = await File.ReadAllTextAsync(openFileDialog.FileName,Encoding.UTF8
                        ,CancellationToken.None);
                    Editor.ClearAll();
                    Editor.Text = file;
                    
                   
                }
            }
            else
            {
                var file = await File.ReadAllTextAsync(filename, Encoding.UTF8
                        , CancellationToken.None);
                Editor.Text = file;
            }
            //UpdatePreview(Editor.Text);

        }
        private   void SaveFile()
        {

            System.Windows.Forms.SaveFileDialog saveFileDialog =
                new System.Windows.Forms.SaveFileDialog();
            DialogResult res = saveFileDialog.ShowDialog();
            saveFileDialog.Filter =
                      "All Files (*.*)|*.*|" +
                          "Markdown (*.md)|*.md|" +
                          "SlimeMarkup (*.smd)|*.smd";
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                   File.WriteAllText (saveFileDialog.FileName, Editor.Text, Encoding.UTF8);
                 
            }

        }
         
        private void Open_Clicked(object sender, EventArgs e)
        {

            OpenFile(null);

        }
        private void Close_Clicked(object sender, EventArgs e)
        {

             Editor.ClearAll();

        }
        private void Save_Clicked(object sender, EventArgs e)
        {

            SaveFile();

        }

        private void H1_Clicked(object sender, EventArgs e)
        {
            if (Editor.SelectedText != null && Editor.SelectedText != String.Empty)
            {
                string selectedtext= Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "#" + selectedtext + "\n");
            }
            else
            {
                Editor.Text += "#Heading 1\n";
            }
        }

        private void H2_Clicked(object sender, EventArgs e)
        {
            if (Editor.SelectedText != null && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "\n## " + selectedtext + "\n");
            }
            else
            {
                Editor.Text += "\n## Heading 2\n";
            }
        }

        private void Bold_Clicked(object sender, EventArgs e)
        {
            if (Editor.SelectedText != null && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "**" + selectedtext + "**");
            }
            else
            {
                Editor.Text += "**bold**";
            }
        }

        private void Italic_Clicked(object sender, EventArgs e)
        {
            if (Editor.SelectedText != null && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext, "*" + selectedtext + "*");
            }
            else
            {
                Editor.Text += "*italic*";
            }
        }

        private void Link_Clicked(object sender, EventArgs e)
        {
            if (Editor.SelectedText != null && Editor.SelectedText != String.Empty)
            {
                string selectedtext = Editor.SelectedText;
                Editor.SelectedText.Replace(selectedtext,
                    "["+selectedtext+"](url){target=_blank rel=nofollow}");
            }
            else
            {
                Editor.Text += "[text](url){target=_blank rel=nofollow}";
            }
        }

        private void Image_Clicked(object sender, EventArgs e)
        {
            if (Editor.SelectedText != null && Editor.SelectedText != String.Empty)
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

        private void List_Clicked(object sender, EventArgs e)
        {
            Editor.Text += "\n- item 1\n- item 2\n";
        }

        private void Table_Clicked(object sender, EventArgs e)
        {
            Editor.Text += """
        | Col1 | Col2 |
        |------|------|
        | A    | B    |
        | C    | D    |
        """;
        }

        private void Quote_Clicked(object sender, EventArgs e)
        {
            if (Editor.SelectedText != null)
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
            Editor.Text += props+"\n";
        }
    }
}