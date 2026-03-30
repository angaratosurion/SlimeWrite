using SlimeWrite.Core;
using SlimeWrite.Core.Models;
using SlimeWrite.MAUI.Helpers;

namespace SlimeWrite.MAUI.Views;

public partial class OptionsView : ContentPage
{
    Kernel core = new Kernel();
    Options options;
    public OptionsView()
    {
        InitializeComponent();
        options= core.GetOptions();
       // this.cbxUseEnterPressedEvent.IsChecked = options.UseEnterPressed;
        this.cbxUseTextChangedEvent.IsChecked = options.UseTextChangedEvent;
        cmbxOrientation.SelectedIndex = options.WebViewOrientation;
        cbxUseUpdateFromGitHub.IsChecked = options.AutoUpdateUsingGithub;
        this.cmbxOrientation.WidthRequest = this.WidthRequest - 50;
       

#if DEBUG
        this.cmbxOrientation.IsVisible = true;
        this.lblOriantation.IsVisible = true;
         this.stLayoutCloseandSave.Margin = new Thickness(50, 0, 0, 0);
        this.HeightRequest = 300;


#endif

    }

    private void Close_Click(object sender, EventArgs e)
    {
        WindowHelper.CloseWindow(this.Window);
    }
    private void Save_Click(object sender, EventArgs e)
    {
         if (options == null)
        {
            options = new Options();
        }
          
        // options.UseEnterPressed = this.cbxUseEnterPressedEvent.IsChecked  ;
        options.UseTextChangedEvent = this.cbxUseTextChangedEvent.IsChecked  ;
        options.AutoUpdateUsingGithub = this.cbxUseUpdateFromGitHub.IsChecked;
        options.WebViewOrientation = cmbxOrientation.SelectedIndex;
        core.SaveOptions(options);
        WindowHelper.CloseWindow(this.Window);
        //Close();
    }
}