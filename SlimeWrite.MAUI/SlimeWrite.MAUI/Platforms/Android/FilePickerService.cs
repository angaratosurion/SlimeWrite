#if ANDROID
using Android.App;
using Android.Content;
using Android.Provider;
using SlimeWrite.MAUI.Platforms.Android;

namespace SlimeWrite.MAUI.Core
{

    public class FilePickerService : IFilePickerService
    {
        TaskCompletionSource<(Stream, string)> _tcs;

        public Task<(Stream stream, string name)> PickFileAsync()
        {
            _tcs = new TaskCompletionSource<(Stream, string)>();

            var activity = Platform.CurrentActivity;

            var intent = new Intent(Intent.ActionOpenDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("*/*");

            activity.StartActivityForResult(intent, 1001);

            return _tcs.Task;
        }

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 1001 && resultCode == Result.Ok)
            {
                var uri = data?.Data;

                if (uri != null)
                {
                    var activity = Platform.CurrentActivity;
                    var stream = activity.ContentResolver.OpenInputStream(uri);

                    string name = "file";

                    var cursor = activity.ContentResolver.Query(uri, null, null, null, null);
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        int index = cursor.GetColumnIndex(OpenableColumns.DisplayName);
                        if (index >= 0)
                            name = cursor.GetString(index);

                        cursor.Close();
                    }

                    _tcs.TrySetResult((stream, name));
                }
            }
        }
    }

}
#endif