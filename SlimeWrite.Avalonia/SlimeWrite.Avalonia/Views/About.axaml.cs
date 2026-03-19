using Avalonia.Controls;
using Avalonia.Interactivity;
using SlimeWrite.Core;
using SlimeWrite.Core.Models;
using System;
using System.Diagnostics;

namespace SlimeWrite.Avalonia;

public partial class About : Window
{
    AppInfo appInfo;
    public About()
    {
        InitializeComponent();
         appInfo = new Kernel().GetAppInfo();

        AppNameText.Text = appInfo.AppName;
        VersionText.Text = $"Version {appInfo.Version}";
        CopyRightText.Text = "© " + DateTime.Now.Year + " " + appInfo.Copyright;
        DescriptionText.Text = appInfo.Description;


        try
        {
            var uri = new Uri("/Assets/Images/logo.png",
                UriKind.Absolute);

        }

        catch { }
    }
    private void WebsiteHyperlink_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(appInfo.Website) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            //MessageBox.Show(this, $"Couldn't open thee Web Site: {ex.Message}",
            //    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();


}