using SlimeWrite.Core;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Models;
using System.Diagnostics;
using AppInfo = SlimeWrite.Core.Models.AppInfo;

namespace SlimeWrite;

public partial class About : ContentPage
{
    AppInfo appInfo;
    public About()
    {
        try
        {
            InitializeComponent();

            appInfo = new Kernel().GetAppInfo();

            AppNameText.Text = appInfo.AppName;
            VersionText.Text = $"Version {appInfo.Version}";
            CopyRightText.Text = "© " + DateTime.Now.Year + " " + appInfo.Copyright;
            DescriptionText.Text = appInfo.Description;
        }
        catch (Exception ex)
        {
            StaticVariables.core.ErrorLog(ex);

             
        }



    }
    private void WebsiteHyperlink_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(appInfo.Website) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            StaticVariables.core.ErrorLog(ex);

            
        }
    }

    private void Close_Click(object sender, EventArgs e)
    {
        try
        {
            // this.Navigation.PopToRootAsync();

            Kernel kernel = new Kernel();
            if (kernel.isDesktopMode())
            {
                WindowHelper.CloseWindow(this.Window);
            }
            else
            {
                WindowHelper.ClosePage(this);
            }
        }
        catch (Exception ex)
        {
            StaticVariables.core.ErrorLog(ex);

             
        }
    }


}
