using SlimeWrite.Core;
using SlimeWrite.Core.Models;

namespace SlimeWrite.MAUI.Views;

public partial class OptionsView : ContentPage
{
    Kernel core = new Kernel();
    Options options;
    public OptionsView()
    {
        InitializeComponent();
        options= core.GetOptions();
        this.cbxUseEnterPressedEvent.IsChecked = options.UseEnterPressed;
        this.cbxUseTextChangedEvent.IsChecked = options.UseTextChangedEvent;
        cmbxOrientation.SelectedIndex = options.WebViewOrientation;
        cbxUseUpdateFromGitHub.IsChecked = options.AutoUpdateUsingGithub;

    }

    private void Close_Click(object sender, EventArgs e)
    {

    }
    private void Save_Click(object sender, EventArgs e)
    {
         if (options == null)
        {
            options = new Options();
        }
          
         options.UseEnterPressed = this.cbxUseEnterPressedEvent.IsChecked ?? false;
        options.UseTextChangedEvent = this.cbxUseTextChangedEvent.IsChecked ?? false;
        options.AutoUpdateUsingGithub = this.cbxUseUpdateFromGitHub.IsChecked ?? false;
        options.WebViewOrientation = cmbxOrientation.SelectedIndex;
        core.SaveOptions(options);

        //Close();
    }
}