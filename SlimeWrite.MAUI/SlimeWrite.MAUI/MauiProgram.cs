using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SlimeWrite.MAUI.Core;
#if ANDROID
using SlimeWrite.MAUI.Platforms.Android;
#endif

namespace SlimeWrite.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .UseMauiCommunityToolkit()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    });
#if ANDROID
            builder.Services.AddSingleton<IFilePickerService, FilePickerService>();
            builder.Services.AddSingleton<FileSaveService>();
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    Android.Util.Log.Error("CRASH", e.ExceptionObject.ToString());
};

TaskScheduler.UnobservedTaskException += (s, e) =>
{
    Android.Util.Log.Error("TASK", e.Exception.ToString());
};
#endif

#if DEBUG
                builder.Logging.AddDebug();
#endif

                return builder.Build();
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                return null;
            }
        }

    }
}
