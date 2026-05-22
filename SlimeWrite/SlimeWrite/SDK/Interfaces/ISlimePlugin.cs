using System;
using System.Collections.Generic;
using System.Text;
using SlimeWrite.Core.Models;
//#if WINDOWS
namespace SlimeWrite.SDK.Interfaces
{
    public interface ISlimePlugin
    {
        string Name { get; }
        string Version { get; }

        // Hooks για τα events της εφαρμογής
        void OnEditorTextChanged(Editor editor, string oldText, 
        string newText, Options options);
        void OnFileOpened(string filePath, ref string fileContent);
        void OnFileSaving(string filePath, ref string fileContent);
    }
}
//#endif