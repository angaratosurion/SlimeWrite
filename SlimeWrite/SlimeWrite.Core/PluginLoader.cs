using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SlimeWrite.Core.SDK.Interfaces;

// #if WINDOWS
namespace SlimeWrite.Core.SDK
{
    public class PluginManager
    {
        public List<ISlimePlugin> Plugins { get; } = new List<ISlimePlugin>();

        public void LoadPlugins(string pluginsDirectory)
        {
            if (!Directory.Exists(pluginsDirectory))
                Directory.CreateDirectory(pluginsDirectory);

            var dllFiles = Directory.GetFiles(pluginsDirectory, "*.dll");

            foreach (var file in dllFiles)
            {
                try
                {
                    // Load assembly
                    Assembly assembly = Assembly.LoadFrom(file);

                    // Find classes that implement ISlimePlugin
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(ISlimePlugin).IsAssignableFrom(type) &&
                            !type.IsInterface &&
                            !type.IsAbstract)
                        {
                            if (Activator.CreateInstance(type) is ISlimePlugin plugin)
                            {
                                Plugins.Add(plugin);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error via your Core system
                    System.Diagnostics.Debug.WriteLine(
                        $"Failed to load plugin {file}: {ex.Message}");
                }
            }
        }
    }
}
// #endif