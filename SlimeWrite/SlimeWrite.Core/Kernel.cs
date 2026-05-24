using SlimeMarkUp.Core;
using SlimeMarkUp.Core.Extensions.SlimeMarkup;
using SlimeWrite.Core.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;
using AppInfo = SlimeWrite.Core.Models.AppInfo;
using Environment = System.Environment;
using SlimeWrite.Core.Helpers;



#if ANDROID

using Android.OS;
#endif

namespace SlimeWrite.Core
{
    public class Kernel
    {
        DocumentManager documentManager = new DocumentManager();
        public Options GetOptions()
        {
            try
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
                {  if (isDesktopMode())
                    {
                        options = new Options
                        {
                            UseTextChangedEvent = true,
                            UseEnterPressed = false,
                            WebViewOrientation = 1,
                            AutoUpdateUsingGithub = true
                        };
                    }
                else
                    {
                        options = new Options
                        {
                            UseTextChangedEvent = true,
                            UseEnterPressed = false,
                            WebViewOrientation = 1,
                            AutoUpdateUsingGithub = false
                        };
                    }
                }
                this.SaveOptions(options);
                return options;
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }

        }
        public AppInfo GetAppInfo()
        {
            try
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
#if ANDROID
                if (Android.App.Application.Context.PackageName.EndsWith("github"))
                {

                    appInfo.AppName = appInfo.AppName.Replace("-GitHubRelease", " [ GitHub Release ]");
                }
#endif



                return appInfo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                // Handle exceptions that may occur during reflection
                Console.WriteLine($"Error retrieving app info: {ex.Message}");
                this.ErrorLog(ex);
                return new AppInfo
                {
                    AppName = "SlimeWrite",
                    Version = "1.0.0",
                };
            }
        }

        public void SaveOptions(Options options)
        {
            try
            {
                string AppDataPath = this.GetAppdataPath();
                string json = JsonSerializer.Serialize(options, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(Path.Combine(AppDataPath, "appsettings.json"), json);
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
               // return null;
            }


        }
        public MarkupParser InitializeParser()
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }



        }
        public string OpenFile(string filename)
        {

            try
            {

                string file = null;

                file = File.ReadAllText(filename, Encoding.UTF8);


                return file;
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }

        }
        public void OpenSegmentedFile(string filename, ref Editor editorText)
        {
            try
            {

                string file = null;
                char[] buffer = new char[1024 * 1024];  // 1MB buffer
                var reader = new StreamReader(filename, Encoding.UTF8);
                int bytesRead;
                //ReadAllText(filename, Encoding.UTF8);//
                editorText.Text = ""; // Καθαρίζει το Text πριν ξεκινήσει η ανάγνωση
                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string chunk = new string(buffer, 0, bytesRead);
                    editorText.Text += chunk;
                    //Task.Delay(50); // Καθυστερεί λίγο την ανανέωση του UI
                }



            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
               // return null;
            }

            //return file;
        }
        
        public string H1_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }

        }
        public string H2_Marked(string selectedtext)
        {

            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }
        }
        public string Bold_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }

        }
        public string Italic_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }


        }
        public string Strikethrough_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }

        }
        public string Link_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }

        }
        public string Image_Marked(string selectedtext, string filename, DocumentInfo documentInfo)
        {
            try
            {
                string ap, image = "";
                if (documentInfo != null)
                {
                    this.documentManager.AddFileToDocumentParentDirectory(documentInfo, filename);
                    image = Path.GetFileName(filename);
                }
                //if (selectedtext != null)
                //{
                //    ap = selectedtext.Replace(selectedtext, "![" + selectedtext +
                //    "](.\\"+image +"){ width = 100 height = 200}");
                //}
                //else
                //{
                ap = "!!alt!!(" + image + "){width=100 height=200}";
                //}
                return ap;
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }
        }
        public string CodeBlock_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }

        }
        public string Quote_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }

        }
        public string List_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }
        }
        public string Table_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }

        }
        public string HorizontalRule_Marked()
        {
            try
            {
                return "\n---\n";
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }
        }
        public string EscapeChars_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }
        }
        public string IncludeCSS_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }
        }
        public string IncludeScript_Marked(string selectedtext)
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }


        }
        public string GetAppdataPath()
        {
            try
            {
                string AppDataPath;
                if (this.isDesktopMode())
                {
                    AppDataPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }
        }
        public string GetTempfolderPath()
        {
            try
            {
                string ap = "";
                string appname = this.GetAppInfo().AppName;
                // if (this.isDesktopMode())
                {

                    ap = Path.Combine(this.GetAppdataPath(), "Cache");
                    if (Directory.Exists(Path.Combine(this.GetAppdataPath(), "Cache")) == false)
                    {
                        Directory.CreateDirectory(Path.Combine(this.GetAppdataPath(), "Cache"));
                    }


                }
                //else
                //    {
                //        ap = Path.Combine(this.GetAppdataPath(), "Cache");
                //        if (Directory.Exists(Path.Combine(this.GetAppdataPath(), "Cache")) == false)
                //        {
                //            Directory.CreateDirectory(Path.Combine(this.GetAppdataPath(), "Cache"));
                //        }

                //    }

                // }
                return ap;
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }

        }
        public string GetLogsFolderPath()
        {
            try
            {
                string ap = "";
                string appname = this.GetAppInfo().AppName;
                if (this.isDesktopMode())
                {
                    ap = Path.Combine(this.GetAppdataPath(), "Logs");
                    if (Directory.Exists(Path.Combine(this.GetAppdataPath(), "Logs")) == false)
                    {
                        Directory.CreateDirectory(Path.Combine(this.GetAppdataPath(), "Logs"));
                    }

                }

                else
                {
#if ANDROID


                    string apppath = this.GetAppdataPath();


                    ap = Path.Combine(apppath, "Logs");
                    if (Directory.Exists(ap) == false)
                    {
                        Directory.CreateDirectory(ap);
                    }
#endif

                }
                return ap;
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }
        }


        public bool isDesktopMode()
        {
            try
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
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return false;

            }
        }
        public void ClearTempFolder()
        {
            try
            {
                string tempFolderPath = this.GetTempfolderPath();
                if (Directory.Exists(tempFolderPath))
                {
                    try
                    {
                        Directory.Delete(tempFolderPath, true);
                        Directory.CreateDirectory(tempFolderPath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        // Handle exceptions that may occur during directory deletion
                        Console.WriteLine($"Error clearing temp folder: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);

            }
        }
        public void ErrorLog(Exception ex)
        {
            string logsFolderPath = this.GetLogsFolderPath();
            string logFilePath = Path.Combine(logsFolderPath, $"log_{DateTime.Now:dd-MM-yyyy}.txt");
            string logEntry = $"[{DateTime.Now:ddd/MM/yyyy HH:mm:ss}] {ex.ToString()}\n";
            try
            {
               
                File.AppendAllText(logFilePath, logEntry);
#if ANDROID
                //string  rellogfile= Path.Combine(GetAppInfo().AppName, "Logs",
                //    Path.GetFileName(logFilePath));
                //    string relogdir = Path.GetDirectoryName(rellogfile);
                //     if (Directory.Exists(relogdir) == false)
                //     {
                //        Directory.CreateDirectory(relogdir);
                //     }
    FileCopier.CopyFileToDownloads(logsFolderPath, logFilePath);

#endif
            }
            catch (Exception logEx)
            {
                System.Diagnostics.Debug.WriteLine(logEx.ToString());
                // Handle exceptions that may occur during logging
                Console.WriteLine($"Error writing to log file: {logEx.Message}");
            }



        }
        public string IncludeFile_Marked(string selectedtext, string filename,
            DocumentInfo documentInfo)
        {
            try
            {
                string ap = "", file = "";
                //if (selectedtext != null)
                //{
                //    ap = selectedtext.Replace(selectedtext, "\n<!-- include :" + selectedtext + "-->\n");
                //}
                //else
                //{
                //    ap = "\n<!-- include  -->\n";
                //}

                if (documentInfo != null)
                {
                    this.documentManager.AddFileToDocumentParentDirectory(documentInfo, filename);
                    file = Path.GetFileName(filename);
                    ap = "\n<!-- include :" + file + "-->\n";
                }
                return ap;
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;
            }


        }
        public string GetPluginsPath()
        {
            try
            {
                string ap = "";

                ap = Path.Combine(this.GetAppdataPath(), "Plugins");
                if (Directory.Exists(ap)==false)
                {
                    Directory.CreateDirectory (ap);
                }

                return ap;
            }
            catch (Exception ex)
            {
                this.ErrorLog(ex);
                return null;

            }
        }
    }
}

