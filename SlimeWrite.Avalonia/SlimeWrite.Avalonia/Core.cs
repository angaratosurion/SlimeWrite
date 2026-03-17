using Avalonia;
using Avalonia.Data;
using SlimeWrite.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SlimeWrite.Avalonia
{
    public class Core
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

    }
}
