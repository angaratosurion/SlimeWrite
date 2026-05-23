using SlimeWrite.Core.Helpers;

namespace SlimeWrite
{
    public partial class AppShell : Shell
    {
        public AppShell()
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

         
    }
}
