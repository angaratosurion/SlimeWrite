using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CompressImages
{
   public class OptionManaer
    {
        public CompressOptions GetOptions()
        {
            try
            {
                CompressOptions ap = null;
                string pluginpath = PluginManager.GetPluginPath("CompressImages",
                    StaticVariables.core.GetPluginsPath());
                if (File.Exists(Path.Combine(pluginpath, "appsettings.json")))
                {
                    using (StreamReader r = new StreamReader(Path.Combine(pluginpath
                        , "appsettings.json")))
                    {
                        string json = r.ReadToEnd();
                        ap = JsonSerializer.Deserialize<CompressOptions>(json);
                    }
                }
                else
                {
                    ap = new CompressOptions();
                    ap.Quality = 75;
                    this.SaveOptions(ap);
                }



                return ap;
            }
            catch (Exception ex)
            {
              StaticVariables.core.ErrorLog(ex);
                return null;
            }
        }
        public void SaveOptions(CompressOptions options)
        {
            try
            {
                string pluginpath = PluginManager.GetPluginPath("CompressImages",
                     StaticVariables.core.GetPluginsPath());
                string json = JsonSerializer.Serialize(options, 
                    new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(Path.Combine(pluginpath, 
                    "appsettings.json"), json);
            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);
                // return null;
            }


        }

    }
}
