
#if IOS
using System;
using System.Collections.Generic;


using System.Text;
using SlimeWrite.Core.IO;
using UIKit;
using UniformTypeIdentifiers;
namespace SlimeWrite.Platforms.iOS
{
   

    public class iOSSaveFileDialog : ISaveFileDialog
    {
        private TaskCompletionSource<string?> _tcs;

        public Task<string?> PickSaveFileAsync(string suggestedName, string[] allowedExtensions)
        {
            _tcs = new();

            var picker = new UIDocumentPickerViewController(
                new string[] { UTTypes.Content.Identifier },
                UIDocumentPickerMode.ExportToService
            );

            picker.DidPickDocument += (s, e) =>
            {
                var url = e.Url?.Path;
                _tcs.TrySetResult(url);
            };

            picker.WasCancelled += (s, e) =>
            {
                _tcs.TrySetResult(null);
            };

            var root = UIApplication.SharedApplication.KeyWindow.RootViewController;
            root.PresentViewController(picker, true, null);

            return _tcs.Task;
        }
    }

}
#endif