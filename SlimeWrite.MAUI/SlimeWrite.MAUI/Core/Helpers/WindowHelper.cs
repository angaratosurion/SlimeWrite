#if WINDOWS
using Microsoft.UI.Xaml;
 
using SlimeWrite.MAUI;
using SlimeWrite.MAUI.Core.Helpers;
 
#endif

#if ANDROID
using Android.App;
 
using AndroidX.Navigation;
using Google.Android.Material.Color.Utilities;
using Microsoft.Maui.Platform;
 
using SlimeWrite.MAUI;
using SlimeWrite.MAUI.Core;
using SlimeWrite.MAUI.Core.Helpers;
using Application = Microsoft.Maui.Controls.Application;
#endif

namespace SlimeWrite.MAUI.Core.Helpers
{
    public static class WindowHelper
    {
       static Kernel core = new Kernel();
        /// <summary>
        /// Κλείνει ένα δευτερεύον Window σε Windows και Android.
        /// Δεν κλείνει το κύριο <c>Window</c>.
        /// </summary>
        public static async void CloseWindow(Microsoft.Maui.Controls.Window window)
        {

            if (window == null)
                return;

            // Προστασία να μην κλείσουμε το κύριο window
#pragma warning disable CS0618 // Type or member is obsolete
            if (window == global::Microsoft.Maui.Controls.Application.Current.MainPage?.Window)
                return;
#pragma warning restore CS0618 // Type or member is obsolete

#if WINDOWS
            var nativeWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            nativeWindow?.Close ();

#endif
            //#if ANDROID        
            //            var activity = window.Handler?.MauiContext?.Context as Android.App.Activity;
            //            activity?.Finish();



            //#endif



        }
        public static async void ClosePage(Page page)
        {


            page.Navigation.PopToRootAsync();


        }
        public static async void OpenWindow(Page page , bool IsMaximizable ,
            bool IsMinimizable)
        {
            if ( page == null)  
                return;
           
            if (core.isDesktopMode())
            {
                var win = new Microsoft.Maui.Controls.Window(page);
                win.IsMaximizable = IsMaximizable;
                win.IsMinimizable = IsMinimizable;
                win.Height = page.HeightRequest;
                win.Width = page.WidthRequest;
                Microsoft.Maui.Controls.Application.Current.OpenWindow(win);
            }
            else
            {
                //About aboutMobile = new About();
                page.Navigation.PushAsync(page);


            }

        }

    }
}