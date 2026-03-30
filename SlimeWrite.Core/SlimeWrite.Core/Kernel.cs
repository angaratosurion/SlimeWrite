

using SlimeMarkUp.Core;
using SlimeMarkUp.Core.Extensions.SlimeMarkup;
using SlimeWrite.Core.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SlimeWrite.Core
{
    public class Kernel
    {
        public Options GetOptions()
        {
            Options options;
            string AppDataPath = this.GetAppdataPath();
            //GetAppInfo().AppName);
            if (File.Exists(Path.Combine(AppDataPath, "appsettings.json")))
            {
                using (StreamReader r = new StreamReader(Path.Combine(AppDataPath, "appsettings.json")))
                {
                    string json = r.ReadToEnd();
                    options = JsonSerializer.Deserialize<Options>(json);
                }
            }
            else
            {
                options = new Options
                {
                    UseTextChangedEvent = true,
                    UseEnterPressed = false,
                    WebViewOrientation = 0,
                    AutoUpdateUsingGithub = true
                };
            }
            this.SaveOptions(options);
            return options;
        }
        public AppInfo GetAppInfo()
        {

            AppInfo appInfo = new AppInfo();

            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            appInfo.AppName = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
                "SlimeWrite";
            appInfo.Version = asm.GetName()?.Version?.ToString() ?? "1.0.0";
            appInfo.Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.
                Copyright;

            appInfo.Description = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.
                Description;



            return appInfo;
        }
        public void SaveOptions(Options options)
        {
            string AppDataPath = this.GetAppdataPath();
            string json = JsonSerializer.Serialize(options, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(Path.Combine(AppDataPath, "appsettings.json"), json);

        }
        public MarkupParser InitializeParser()
        {
            MarkupParser _parser;

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


            return _parser;


        }
        public string OpenFile(string filename)
        {
            string file = File.ReadAllText(filename, Encoding.UTF8);

            return file;
        }
        public void SaveFile(string filename, string content)
        {
            File.WriteAllText(filename, content, Encoding.UTF8);
        }
        public string H1_Marked(string selectedtext)
        {
            string ap;

            if (selectedtext != null)
            {

                ap = selectedtext.Replace(selectedtext, "#" + selectedtext + "\n");

            }
            else
            {
                ap = "#Heading 1\n";
            }

            return ap;
        }
        public string H2_Marked(string selectedtext)
        {
            string ap;

            if (selectedtext != null)
            {

                ap = selectedtext.Replace(selectedtext, "\n## " + selectedtext + "\n");

            }
            else
            {
                ap = "\n## Heading 2\n";
            }

            return ap;
        }
        public string Bold_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "**" + selectedtext + "**");
            }
            else
            {
                ap = "**Bold Text**";
            }
            return ap;
        }
        public string Italic_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "*" + selectedtext + "*");
            }
            else
            {
                ap = "*Italic Text*";
            }
            return ap;

        }
        public string Strikethrough_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "~~" + selectedtext + "~~");
            }
            else
            {
                ap = "~~Strikethrough Text~~";
            }
            return ap;

        }
        public string Link_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext,
                "[" + selectedtext + "](url){target=_blank rel=nofollow}");
            }
            else
            {
                ap = "[text](url){target=_blank rel=nofollow}";
            }
            return ap;

        }
        public string Image_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "![" + selectedtext +
                "](image.png){ width = 100 height = 200}");
            }
            else
            {
                ap = "![alt](image.png){width=100 height=200}";
            }
            return ap;

        }
        public string CodeBlock_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "`\n" + selectedtext + "\n`");
            }
            else
            {
                ap = "`\nCode Block\n`";
            }
            return ap;

        }
        public string Quote_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "\n>" + selectedtext + "\n");
            }
            else
            {
                ap = "\n> quote\n";
            }
            return ap;

        }
        public string List_Marked(string selectedtext)
        {
            string ap;
            //if (selectedtext != null)
            //{
            //    ap = selectedtext.Replace(selectedtext, "\n- " + selectedtext + "\n");
            //}
            //else
            //{
            ap = "\n- item 1\n- item 2\n";
            //}
            return ap;
        }
        public string Table_Marked(string selectedtext)
        {
            string ap;
            //if (selectedtext != null)
            //{
            //    ap = selectedtext.Replace(selectedtext, "\n| Header 1 | Header 2 |\n| --- | --- |\n| Cell 1 | Cell 2 |\n");
            //}
            //else
            //{
            ap = "\n" + """
        | Col1 | Col2 |
        |------|------|
        | A    | B    |
        | C    | D    |
        """;
            //}
            return ap;

        }
        public string HorizontalRule_Marked()
        {
            return "\n---\n";
        }
        public string EscapeChars_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, @"\" + selectedtext + @"\");
            }
            else
            {
                ap = @"\ Escaped Text\";
            }
            return ap;
        }
        public string IncludeCSS_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "\n<!-- include style:" + selectedtext + "-->\n");
            }
            else
            {
                ap = "\n<!-- include style: style.css -->\n";
            }
            return ap;
        }
        public string IncludeScript_Marked(string selectedtext)
        {
            string ap;
            if (selectedtext != null)
            {
                ap = selectedtext.Replace(selectedtext, "\n<!-- include script:" + selectedtext + "-->\n");
            }
            else
            {
                ap = "\n<!-- include script: script.js -->\n";
            }
            return ap;


        }
        public string GetAppdataPath()
        {
            string AppDataPath;
            if (this.isDesktopMode())
            {
                AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    GetAppInfo().AppName);
            }
            else
            {
                AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            }

            if (Directory.Exists(AppDataPath) == false)
            {
                Directory.CreateDirectory(AppDataPath);
            }

            return AppDataPath;
        }
        public bool isDesktopMode()
        {
            bool isDesktop = false;


            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            {
                isDesktop = false;
            }
            else if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                isDesktop = true;
            }
            return isDesktop;
        }
         


    }

}

