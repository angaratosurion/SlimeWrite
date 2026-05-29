using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS
using SlimeWrite.Core.IO;
using Windows.Storage.Pickers;
using WinRT;
using WinRT.Interop;

namespace SlimeWrite.Platforms.Windows
{
    

    public class WindowsSaveFileDialog : ISaveFileDialog
    {
        public async Task<string?> PickSaveFileAsync(string suggestedName, string[] allowedExtensions)
        {
            var picker = new FileSavePicker();
            picker.SuggestedFileName = suggestedName;

            foreach (var ext in allowedExtensions)
                picker.FileTypeChoices.Add(ext.ToUpper(), new List<string> { ext });

            var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView)
                .WindowHandle;

            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            return file?.Path;
        }
    }

}
#endif