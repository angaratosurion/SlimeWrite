using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel; // Permissions
#if ANDROID
using Android.Content;
using Android.App;
using Android.Net;
using Android.OS;
#endif

namespace SlimeWrite.MAUI.Core.Helpers
{
    public static class StoragePermissionHelper
    {
        /// <summary>
        /// Ελέγχει ή ζητάει άδεια πρόσβασης σε εξωτερική αποθήκευση/Media.
        /// </summary>
        public static async Task<bool> CheckAndRequestStoragePermissionAsync()
        {
            try {
#if ANDROID
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                // Android 11+ χρειάζεται special permission για όλο το storage
                if (!Android.OS.Environment.IsExternalStorageManager)
                {
                    OpenAppSettings(); // ανοίγει τις ρυθμίσεις για MANAGE_EXTERNAL_STORAGE
                    return false;
                }
                return true;
            }
            else
            {
                // Android 10 και παλαιότερα: runtime άδεια READ/WRITE
                var statusRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                var statusWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                if (statusRead != PermissionStatus.Granted || statusWrite != PermissionStatus.Granted)
                {
                    statusRead = await Permissions.RequestAsync<Permissions.StorageRead>();
                    statusWrite = await Permissions.RequestAsync<Permissions.StorageWrite>();
                }

                return statusRead == PermissionStatus.Granted && statusWrite == PermissionStatus.Granted;
            }
#else
                // Άλλες πλατφόρμες δεν απαιτούν
                return true;
#endif
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                 return false;
            }
        }


#if ANDROID
        /// <summary>
        /// Ανοίγει τις ρυθμίσεις της εφαρμογής (App Settings)
        /// </summary>
        public static void OpenAppSettings()
        {
         try
         {
            var context = Android.App.Application.Context;
            Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.SetData(Android.Net.Uri.Parse($"package:{context.PackageName}"));
            intent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
            }
             catch (Exception ex)
            { 
                MainPage.core.ErrorLog(ex);
                
                 
            }
        }
#endif

    }
}

