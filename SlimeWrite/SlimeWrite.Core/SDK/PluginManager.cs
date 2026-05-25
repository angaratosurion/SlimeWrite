using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SlimeWrite.Archive;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.SDK.Interfaces;

// #if WINDOWS
namespace SlimeWrite.Core.SDK
{
    public class PluginManager
    {
        public List<ISlimePlugin> Plugins { get; } =
            new List<ISlimePlugin>();


        public void LoadPlugins(string pluginsDirectory)
        {
            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);

            // Βρες όλους τους υποφακέλους (κάθε ένας = plugin)
            var pluginFolders = Directory.GetDirectories(pluginsDirectory);

            foreach (var folder in pluginFolders)
            {
                // Βρες ΟΛΑ τα DLL μέσα στον φάκελο
                var dllFiles = Directory.GetFiles(folder, "*.dll",
                    SearchOption.TopDirectoryOnly);

                foreach (var dll in dllFiles)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(dll);

                        foreach (Type type in assembly.GetTypes())
                        {
                            if (typeof(ISlimePlugin).IsAssignableFrom(type)
                                &&
                                !type.IsInterface &&
                                !type.IsAbstract)
                            {
                                if (Activator.CreateInstance(type) is 
                                    ISlimePlugin plugin)
                                {
                                    Plugins.Add(plugin);

                                    System.Diagnostics.Debug.WriteLine(
                                        $"Loaded plugin '{plugin.Name}' from {dll}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticVariables.core.ErrorLog(ex);
                        
                    }
                }
            }
        }

        public void InstallPlugin(string pluginPath, string pluginsDirectory)
        {
            try
            {
                if (!Directory.Exists(pluginsDirectory))
                    Directory.CreateDirectory(pluginsDirectory);
                string fileName = Path.GetFileName(pluginPath);
                string destPath = Path.Combine(pluginsDirectory, 
                    Path.GetFileNameWithoutExtension(fileName));
                 Slime7z.Extract(pluginPath, destPath);
                LoadPlugins(pluginsDirectory);

            }
            catch (Exception ex)
            {
               StaticVariables.core.ErrorLog(ex);
            }
        }
    }
}
// #endif