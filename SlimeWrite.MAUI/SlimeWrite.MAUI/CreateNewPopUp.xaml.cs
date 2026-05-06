using CommunityToolkit.Maui.Views;

namespace SlimeWrite.MAUI;

public  partial class CreateNewDocumentPopUp : Popup<string>
{
    public string Result { get; private set; }

    public CreateNewDocumentPopUp()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);

            
        }
    }

    void OnOkClicked(object sender, EventArgs e)
    {
        try
        {
            Result = DocumentName.Text;
            this.CloseAsync(Result);

        }
        catch (Exception ex)
        {
            MainPage.core.ErrorLog(ex);

             Result = null;
        }

    }
}