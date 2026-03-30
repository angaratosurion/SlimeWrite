using Microsoft.Maui.Controls;

#if WINDOWS
using Microsoft.UI.Xaml;
using SlimeWrite;
using SlimeWrite.MAUI;
using SlimeWrite.MAUI.Helpers;
#endif

#if ANDROID
using Microsoft.Maui.Platform;
using Android.App;
using SlimeWrite;
using SlimeWrite.MAUI;
using SlimeWrite.MAUI.Helpers;
#endif

namespace SlimeWrite.MAUI.Helpers
{
    public static class WindowHelper
    {
        /// <summary>
        /// Κλείνει ένα δευτερεύον Window σε Windows και Android.
        /// Δεν κλείνει το κύριο <c>Window</c>.
        /// </summary>
        public static void CloseWindow(Microsoft.Maui.Controls.Window window)
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
#elif ANDROID
    var activity = window.Handler?.MauiContext?.Context as Android.App.Activity;
    activity?.Finish();
#endif
        }
    }
}