using ScintillaNET;
using SlimeMarkUp.Core;
using SlimeMarkUp.Core.Extensions.SlimeMarkup;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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
        private bool _webViewReady = false;
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
                new IncludeExtension()
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
            _markdown = Editor.Text ?? "";
            UpdatePreview(_markdown);
        }

        private async Task UpdatePreview(string markdown)
        {
            var md = _parser.Parse(markdown);
            var html = "<html>\r\n<head>\r\n <meta charset=\"UTF-8\" /></head><style>\r\n                        " +
                "body {\r\n    color:black; }  </style>" +
                " <body>" +
                _renderer.Render(md) + "</body>\r\n</html>";
          
             

            
             
            Preview.NavigateToString(html);


        }
        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            _markdown = Editor.Text ?? "";
            UpdatePreview(_markdown);
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
                    var file = await File.ReadAllTextAsync(openFileDialog.FileName);
                    Editor.Text = file;
                }
            }
            else
            {
                var file = await File.ReadAllTextAsync(filename);
                Editor.Text = file;
            }

        }
        private async void SaveFile()
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
                   File.WriteAllText (saveFileDialog.FileName, Editor.Text);
                 
            }

        }
        public async Task<string> StreamToStringAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
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
            Editor.Text += "#Heading 1\n";
        }

        private void H2_Clicked(object sender, EventArgs e)
        {
            Editor.Text += "\n## Heading 2\n";
        }

        private void Bold_Clicked(object sender, EventArgs e)
        {
            Editor.Text += "**bold**";
        }

        private void Italic_Clicked(object sender, EventArgs e)
        {
            Editor.Text += "*italic*";
        }

        private void Link_Clicked(object sender, EventArgs e)
        {
            Editor.Text += "[text](url){target=_blank rel=nofollow}";
        }

        private void Image_Clicked(object sender, EventArgs e)
        {
            Editor.Text += "![alt](image.png){width=100 height=200}";
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
            Editor.Text += "\n> quote\n";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}