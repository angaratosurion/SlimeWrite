using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeWrite.Core.IO
{
    public interface ISaveFileDialog
    {
        Task<string?> PickSaveFileAsync(
            string suggestedName,
            string[] allowedExtensions
        );
    }

}
