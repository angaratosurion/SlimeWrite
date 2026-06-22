using SlimeWrite.Core;
using SlimeWrite.Core.Helpers;

namespace CompressImages;

public partial class ImgCompressOptions : ContentPage
{

    CompressOptions options = null;
	public ImgCompressOptions()
	{
		InitializeComponent();
        var optmnger = new OptionManaer();
        options = optmnger.GetOptions();
        this.sldQuality.Value= options.Quality;
        
    }

    private void brnCancel_Clicked(object sender, EventArgs e)
    {
        try
        {
            Kernel kernel = StaticVariables.core;
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

    private void btnSave_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (options == null)
            {
                options = new CompressOptions();
            }
            options.Quality = (uint)this.sldQuality.Value;
            var optmnger = new OptionManaer();
            optmnger.SaveOptions(options);
            Kernel kernel = StaticVariables.core;
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