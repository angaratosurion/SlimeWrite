using SlimeWrite.Core;
using SlimeWrite.Core.Helpers;
using SlimeWrite.Core.Models;
using SlimeWrite.Core.SDK;

namespace SlimeWrite.Views
{

    public partial class OptionsView : ContentPage
    {
        Kernel core = new Kernel();
        Options options;
        public OptionsView()
        {
            try
            {
                InitializeComponent();
                options = core.GetOptions();
                this.SetEventRefresh();
                //this.cbxUseEnterPressedEvent.IsChecked = options.UseEnterPressed;
                //this.cbxUseTextChangedEvent.IsChecked = options.UseTextChangedEvent;
                cmbxOrientation.SelectedIndex = options.WebViewOrientation;
                cbxUseUpdateFromGitHub.IsChecked = options.AutoUpdateUsingGithub;
                this.cmbxOrientation.WidthRequest = this.WidthRequest - 50;
                //this.cbxSegmentedLoading.IsChecked = options.SegmentedLoading;
                //this.txtMaxSegmentLength.Text = Convert.ToString(options.MaxSegmentLength);
#if ANDROID
                this.btnSetPerimitions.IsVisible = true;
                // this.cbxUseUpdateFromGitHub.IsVisible = false;
                this.stcGitHub.IsVisible = false;

#endif



#if DEBUG
                /* this.cmbxOrientation.IsVisible = true;
                 this.lblOriantation.IsVisible = true;
                 this.stLayoutCloseandSave.Margin = new Thickness(50, 0, 0, 0);
                 if (core.isDesktopMode())
                 {
                     this.HeightRequest = 300*5;
                 }
                 else
                 {
                     this.HeightRequest = new GridLength(500*2,
                         GridUnitType.Star).Value;
                 }

                 */


#endif
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }

        }

        private void Close_Click(object sender, EventArgs e)
        {
            try
            {
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
                MainPage.core.ErrorLog(ex);


            }
        }
        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (options == null)
                {
                    options = new Options();
                }
                //this.getEventRefresh();
                this.SetEventRefresh();
                //options.UseEnterPressed = this.cbxUseEnterPressedEvent.IsChecked  ;
                //options.UseTextChangedEvent = this.cbxUseTextChangedEvent.IsChecked;
                options.AutoUpdateUsingGithub = this.cbxUseUpdateFromGitHub.IsChecked;
                options.WebViewOrientation = cmbxOrientation.SelectedIndex;
                //options.SegmentedLoading = this.cbxSegmentedLoading.IsChecked;
                //options.MaxSegmentLength = int.Parse(this.txtMaxSegmentLength.Text);
                core.SaveOptions(options);
                //  WindowHelper.CloseWindow(this.Window);
                Kernel kernel = new Kernel();
                if (kernel.isDesktopMode())
                {
                    WindowHelper.CloseWindow(this.Window);
                }
                else
                {
                    WindowHelper.ClosePage(this);
                }
                //Close();
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);


            }
        }
        public void SetEventRefresh()
        {
            try
            {
                if (this.cmbxEventChange.SelectedIndex == 0)
                {
                    options.UseTextChangedEvent = true;
                    options.UpdateOnLosingFocus = false;
                }
                else
                {
                    options.UseTextChangedEvent = false;
                    options.UpdateOnLosingFocus = true;
                }

            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

            }
        }
        public void getEventRefresh()
        {
            try
            {
                if (options.UpdateOnLosingFocus && options.UseTextChangedEvent == false)
                {
                    this.cmbxEventChange.SelectedIndex = 1;
                }
                else if (options.UpdateOnLosingFocus == false && options.UseTextChangedEvent)
                {
                    this.cmbxEventChange.SelectedIndex = 1;
                }
                else if (options.UpdateOnLosingFocus && options.UseTextChangedEvent)
                {
                    this.cmbxEventChange.SelectedIndex = 0;
                }
                else
                {
                    this.cmbxEventChange.SelectedIndex = 0;
                }
            }


            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                // return null;
            }
        }
        private async void btnInstallPlugins_Clicked(object sender, EventArgs e)
        {
            try
            {
                PickOptions pickOptions = new PickOptions();
                var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
{
    { DevicePlatform.WinUI, new[] { ".7z" } },
    { DevicePlatform.Android, new[] { "application/x-7z-compressed" } },
    { DevicePlatform.iOS, new[] { "org.7-zip.7-zip-archive" } },
    { DevicePlatform.MacCatalyst, new[] { "org.7-zip.7-zip-archive" } }
});
                string filename = null;
                var res = await FilePicker.Default.PickAsync(pickOptions);

                if (res != null)
                {
                    filename = res.FullPath;
                    PluginManager.InstallPlugin(filename,
                        StaticVariables.core.GetPluginsPath());


                }
                else
                {
                    return; // Ο χρήστης ακύρωσε το pick
                }
            }
            catch (Exception ex)
            {
                StaticVariables.core.ErrorLog(ex);

            }
        }


        private void btnSetPerimitions_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool granted = StoragePermissionHelper.CheckAndRequestStoragePermissionAsync().Result;
                if (!granted)
                {
                    // Ενημέρωση χρήστη για την ανάγκη άδειας
                    DisplayAlert("Permission Required", "Storage access is required to open and save files.", "OK");
                    // StoragePermissionHelper.OpenAppSettings();
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                // return null;
            }

        }
    }
}