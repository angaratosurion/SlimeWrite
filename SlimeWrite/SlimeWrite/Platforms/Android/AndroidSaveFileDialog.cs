#if ANDROID
using Android.Content;
using SlimeWrite.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeWrite.Platforms.Android
{
    public class AndroidSaveFileDialog : ISaveFileDialog
    {
        private TaskCompletionSource<string?> _tcs;
        public Task<string?> PickSaveFileAsync(string suggestedName, string[] allowedExtensions)
        {
            _tcs = new TaskCompletionSource<string?>();

            AndroidSaveFileDialogHandler.SetTask(_tcs);

            var intent = new Intent(Intent.ActionCreateDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("*/*");
            intent.PutExtra(Intent.ExtraTitle, suggestedName);

            Platform.CurrentActivity.StartActivityForResult(intent, 999);

            return _tcs.Task;
        }
      
    }

}
#endif