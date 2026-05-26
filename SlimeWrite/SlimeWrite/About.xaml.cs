using SlimeWrite.Core;
using SlimeWrite.Core.Helpers;
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
    private void PluginsList_Click(object sender, EventArgs e)
    {
        try
        {
            PluginsList pluginsList = new PluginsList();
            if (StaticVariables.core.isDesktopMode())
            {
                var win = new Window(pluginsList)
                {
                    IsMaximizable = false,
                    IsMinimizable = false,
                    Height = pluginsList.HeightRequest,
                    Width = pluginsList.WidthRequest
                };
                Application.Current?.OpenWindow(win);
            }
            else
            {
                this.Navigation.PushAsync(pluginsList);
            }


        }
        catch (Exception ex)
        {
            StaticVariables.core.ErrorLog(ex);


        }
    }


}
