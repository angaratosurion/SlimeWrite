using SlimeWrite.Core.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SlimeWrite.Core.Models.ViewModels
{
    public partial class PluginsViewModel : ObservableObject
    {
        // Η λίστα με τα plugins σου
       // public ObservableCollection<ISlimePlugin> InstalledPlugins { get; set; } = new();

        [RelayCommand]
        private void OpenSettings(ISlimePlugin plugin)
        {
            if (plugin != null)
            {
                // Παίρνεις την τρέχουσα σελίδα της εφαρμογής για να την περάσεις στο interface
                var mainPage = Application.Current.MainPage;

                // Καλείς τη μέθοδο του interface
                plugin.OpenPluginSettings(mainPage as ContentPage);
            }
        }
    }
}
