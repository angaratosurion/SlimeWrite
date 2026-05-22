using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

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
            ////    MainPage.core.ErrorLog(ex);


            ////}
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                // 🔴 Βάλε ένα BREAKPOINT σε αυτή τη γραμμή!
                var exception = e.ExceptionObject as Exception;

                // Αυτό θα εκτυπώσει το πραγματικό σφάλμα στο Output Window του Visual Studio
                Debug.WriteLine($"==========================================");
                Debug.WriteLine($"CRITICAL CROSS-PLATFORM CRASH: {exception?.Message}");
                Debug.WriteLine($"STACK TRACE: {exception?.StackTrace}");
                Debug.WriteLine($"==========================================");
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //try
            //{
                return new Window(new AppShell());
            //}
            //catch (Exception ex)
            //{
            //    MainPage.core.ErrorLog(ex);

            //    return null;
            //}
        }
    }
}