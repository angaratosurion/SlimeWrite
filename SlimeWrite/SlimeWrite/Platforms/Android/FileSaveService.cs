using Android.App;
using Android.Content;
using Android.Net;
using Microsoft.Maui.ApplicationModel;
 
using System.Collections.Generic;
using System.Text;
using Uri = Android.Net.Uri;

namespace SlimeWrite.Platforms.Android
{
    public class FileSaveService
    {
        TaskCompletionSource<(Uri uri, bool ok)> _tcs;
        Stream _stream;

        const int RequestCode = 5001;

        public Task<bool> SaveAsync(Stream stream, string suggestedName)
        {
            try
            {
                _stream = stream;
                _tcs = new TaskCompletionSource<(Uri, bool)>();

                var activity = Platform.CurrentActivity;

                var intent = new Intent(Intent.ActionCreateDocument);
                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("*/*");
                intent.PutExtra(Intent.ExtraTitle, suggestedName);

                activity.StartActivityForResult(intent, RequestCode);

                return WaitForResult();
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                return null;
            }
        }

        public async Task<bool> WaitForResult()
        {  try
            {
                var result = await _tcs.Task;

                if (!result.ok || result.uri == null)
                    return false;

                var activity = Platform.CurrentActivity;

                using var output = activity.ContentResolver.OpenOutputStream(result.uri);
                await _stream.CopyToAsync(output);

                return true;
            
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);
                return false;

            }
        }

        public void OnResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                if (requestCode != RequestCode)
                    return;

                if (resultCode == Result.Ok && data?.Data != null)
                {
                    _tcs.TrySetResult((data.Data, true));
                }
                else
                {
                    _tcs.TrySetResult((null, false));
                }
            }
            catch (Exception ex)
            {
                MainPage.core.ErrorLog(ex);

                
            }
        }
    }
}
