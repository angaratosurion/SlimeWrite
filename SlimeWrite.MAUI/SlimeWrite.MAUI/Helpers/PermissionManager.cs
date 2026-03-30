using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

#if ANDROID
using Android.App;
using Android.Content.PM;
using Microsoft.Maui.ApplicationModel;
#endif

#if WINDOWS
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Media.Capture;
using Windows.Devices.Geolocation;
#endif

#if IOS
using Foundation;
using Microsoft.Maui.ApplicationModel;
#endif

namespace SlimeWrite.MAUI.Helpers
{
    public static class PermissionManager
    {
        public static async Task RequestAllDeclaredAsync()
        {
#if ANDROID
            var permissions = GetAndroidDeclaredPermissions();
            foreach (var perm in permissions)
            {
                await RequestRuntimePermissionAndroid(perm);
            }
#endif

#if WINDOWS
            var caps = await GetWindowsDeclaredCapabilitiesAsync();
            foreach (var cap in caps)
            {
                await RequestRuntimePermissionWindows(cap);
            }
#endif

#if IOS
            var caps = GetIOSDeclaredPermissions();
            foreach (var cap in caps)
            {
                await RequestRuntimePermissionIOS(cap);
            }
#endif
        }

#if ANDROID
        private static List<string> GetAndroidDeclaredPermissions()
        {
            var list = new List<string>();
            var context = Android.App.Application.Context;

            var info = context.PackageManager.GetPackageInfo(
                context.PackageName,
                PackageInfoFlags.Permissions);

            if (info?.RequestedPermissions != null)
                list.AddRange(info.RequestedPermissions);

            return list;
        }

        private static async Task RequestRuntimePermissionAndroid(string permission)
        {
            var type = MapAndroidPermissionToMaui(permission);
            if (type != null)
            {
                // CheckStatusAsync<T>
                var checkMethod = typeof(Permissions).GetMethod(nameof(Permissions.CheckStatusAsync))
                    .MakeGenericMethod(type);
                var task = (Task<PermissionStatus>)checkMethod.Invoke(null, null);
                var status = await task;

                if (status != PermissionStatus.Granted)
                {
                    var reqMethod = typeof(Permissions).GetMethod(nameof(Permissions.RequestAsync))
                        .MakeGenericMethod(type);
                    var reqTask = (Task<PermissionStatus>)reqMethod.Invoke(null, null);
                    await reqTask;
                }
            }
        }

        private static Type MapAndroidPermissionToMaui(string permission)
        {
            var map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["android.permission.CAMERA"] = typeof(Permissions.Camera),
                ["android.permission.ACCESS_FINE_LOCATION"] = typeof(Permissions.LocationWhenInUse),
                ["android.permission.ACCESS_COARSE_LOCATION"] = typeof(Permissions.LocationWhenInUse),
                ["android.permission.RECORD_AUDIO"] = typeof(Permissions.Microphone),
                ["android.permission.READ_EXTERNAL_STORAGE"] = typeof(Permissions.StorageRead),
                ["android.permission.WRITE_EXTERNAL_STORAGE"] = typeof(Permissions.StorageWrite),
                ["android.permission.POST_NOTIFICATIONS"] = typeof(Permissions.PostNotifications)
            };

            map.TryGetValue(permission, out var t);
            return t;
        }
#endif

#if WINDOWS
        private static async Task<List<string>> GetWindowsDeclaredCapabilitiesAsync()
        {
            var list = new List<string>();
            var file = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");
            var xml = await XmlDocument.LoadFromFileAsync(file);

            foreach (var tag in new[] { "Capability", "DeviceCapability" })
            {
                var nodes = xml.GetElementsByTagName(tag);
                foreach (var node in nodes)
                {
                    var name = node.Attributes.GetNamedItem("Name")?.NodeValue?.ToString();
                    if (!string.IsNullOrEmpty(name))
                        list.Add(name);
                }
            }
            return list;
        }

        private static async Task RequestRuntimePermissionWindows(string cap)
        {
            switch (cap.ToLower())
            {
                case "webcam":
                    try
                    {
                        var capture = new MediaCapture();
                        await capture.InitializeAsync(new MediaCaptureInitializationSettings
                        {
                            StreamingCaptureMode = StreamingCaptureMode.Video
                        });
                    }
                    catch { }
                    break;

                case "microphone":
                    try
                    {
                        var capture = new MediaCapture();
                        await capture.InitializeAsync();
                    }
                    catch { }
                    break;

                case "location":
                    await Geolocator.RequestAccessAsync();
                    break;
            }
        }
#endif

#if IOS
        private static List<string> GetIOSDeclaredPermissions()
        {
            var list = new List<string>();
            var plistPath = Path.Combine(NSBundle.MainBundle.BundlePath, "Info.plist");
            var dict = NSDictionary.FromFile(plistPath);

            foreach (var key in dict.Keys)
            {
                if (key.ToString().EndsWith("UsageDescription"))
                    list.Add(key.ToString());
            }
            return list;
        }

        private static async Task RequestRuntimePermissionIOS(string usageKey)
        {
            if (usageKey == "NSCameraUsageDescription")
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                    await Permissions.RequestAsync<Permissions.Camera>();
            }
            else if (usageKey == "NSMicrophoneUsageDescription")
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
                if (status != PermissionStatus.Granted)
                    await Permissions.RequestAsync<Permissions.Microphone>();
            }
            else if (usageKey == "NSLocationWhenInUseUsageDescription")
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }
#endif
    }
}