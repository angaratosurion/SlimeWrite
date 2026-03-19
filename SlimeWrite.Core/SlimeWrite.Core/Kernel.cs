
using SlimeMarkUp.Core;
using SlimeMarkUp.Core.Extensions.SlimeMarkup;
using SlimeWrite.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            using (StreamReader r = new StreamReader("appsettings.json"))
            {
                string json = r.ReadToEnd();
                options = JsonSerializer.Deserialize<Options>(json);
            }
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
            string json = JsonSerializer.Serialize(options, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText("appsettings.json", json);

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
            string  file =   File.ReadAllText(filename, Encoding.UTF8);

            return file;
        }
        public void SaveFile(string filename, string content)
        {
            File.WriteAllText(filename, content, Encoding.UTF8);
        }

    }
}
