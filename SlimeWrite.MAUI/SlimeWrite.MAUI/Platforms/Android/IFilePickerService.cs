using System;
using System.Collections.Generic;
using System.Text;
namespace SlimeWrite.MAUI.Platforms.Android
{
    #if ANDROID

    public interface IFilePickerService
    {
        Task<(Stream stream, string name)> PickFileAsync();
    }
#endif
}
