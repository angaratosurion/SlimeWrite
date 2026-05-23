using Microsoft.Extensions.DependencyInjection;

namespace SlimeWrite
{
    public partial class App : Application
    {
        public App()
        {
            //try
            //{
                InitializeComponent();
            //}
            ////catch (Exception ex)
            ////{
            ////    StaticVariables.core.ErrorLog(ex);

                
            ////}
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //try
            //{
                return new Window(new AppShell());
            //}
            //catch (Exception ex)
            //{
            //    StaticVariables.core.ErrorLog(ex);

            //    return null;
            //}
        }
    }
}