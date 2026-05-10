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
                MainPage.core.ErrorLog(ex);

                 
            }
        }

         
    }
}
