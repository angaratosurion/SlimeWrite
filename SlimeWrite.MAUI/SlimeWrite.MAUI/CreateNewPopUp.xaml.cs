using CommunityToolkit.Maui.Views;

namespace SlimeWrite.MAUI;

public  partial class CreateNewDocumentPopUp : Popup<string>
{
    public string Result { get; private set; }

    public CreateNewDocumentPopUp()
    {
        InitializeComponent();
    }

    void OnOkClicked(object sender, EventArgs e)
    {
        Result = DocumentName.Text;
        this.CloseAsync(Result);
        
    }
}