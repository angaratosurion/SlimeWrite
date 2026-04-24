using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Google.Android.Material.Color.Utilities;
using Java.Nio.FileNio.Attributes;
using SlimeWrite.MAUI.Platforms.Android;
using static Bumptech.Glide.DiskLruCache.DiskLruCache;
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaAudio)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaVideo)]

namespace SlimeWrite.MAUI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);

        //    var service = MauiApplication.Current.Services.GetService<FileSaveService>();
        //    service?.OnResult(requestCode, resultCode, data!);
        //    var options= MainPage.core.GetOptions();
           
           
        //}
    }
}
