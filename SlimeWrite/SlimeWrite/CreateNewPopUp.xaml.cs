using CommunityToolkit.Maui.Views;
using SlimeWrite.Core.Helpers;

namespace SlimeWrite;

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
            StaticVariables.core.ErrorLog(ex);

            
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
            StaticVariables.core.ErrorLog(ex);

             Result = null;
        }

    }
}