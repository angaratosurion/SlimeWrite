using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
//#if WINDOWS
namespace SlimeWrite.SDK.Interfaces
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
                    // Φόρτωση του Assembly
                    Assembly assembly = Assembly.LoadFrom(file);
                    
                    // Εύρεση των κλάσεων που υλοποιούν το ISlimePlugin
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(ISlimePlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
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
                    // Log το σφάλμα μέσω του core σου
                    System.Diagnostics.Debug.WriteLine($"Failed to load plugin {file}: {ex.Message}");
                }
            }
        }
    }
}
//#endif
