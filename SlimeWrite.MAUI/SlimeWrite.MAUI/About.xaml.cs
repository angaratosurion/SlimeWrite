using SlimeWrite.Core;
using SlimeWrite.Core.Models;
using System.Diagnostics;
using AppInfo = SlimeWrite.Core.Models.AppInfo;

namespace SlimeWrite.MAUI;

public partial class About : ContentPage
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

 
        
    }
    private void WebsiteHyperlink_Click(object sender, EventArgs e)
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

    private void Close_Click(object sender, EventArgs e)
    {
          WindowHelper.CloseWindow(this.Window);

    }


}
