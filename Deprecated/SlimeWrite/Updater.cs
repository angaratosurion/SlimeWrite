using SlimeWrite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SlimeWrite
{
   public class Updater
    {   

        public async Task DownloadLatestRelease()
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var AppName = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
                "SlimeWrite";
           var  Version = asm.GetName()?.Version?.ToString() ?? "1.0.0";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(AppName, Version));

            var url = "https://api.github.com/repos/angaratosurion/SlimeWrite/releases/latest";
            var json = await client.GetStringAsync(url);

            var release = JsonSerializer.Deserialize<GitHubRelease>(json);
            if (release != null)     
            {
                 var ver= new Version(release.tag_name.TrimStart('v'));
                var appVer = new Version(Version);
                if (ver > appVer)
                { 

                    var asset = release.assets.FirstOrDefault(a => a.name.EndsWith(".exe"));

                    if (asset != null)
                    {
                        var bytes = await client.GetByteArrayAsync(asset.browser_download_url);
                        var file = Path.Combine(Path.Combine(Path.GetTempPath(),
                            "SlimeWrite"), "latst.exe");
                        if (!File.Exists(file))
                        {


                            await File.WriteAllBytesAsync(file, bytes);
                        }
                        else
                        {
                            File.Delete(file);
                        }
                        Process.Start(file);
                    }
                }
            }
        }
    }
}
