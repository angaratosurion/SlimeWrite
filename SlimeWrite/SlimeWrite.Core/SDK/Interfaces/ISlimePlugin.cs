using System;
using System.Collections.Generic;
using System.Text;
using SlimeWrite.Core.Models;

// #if WINDOWS
namespace SlimeWrite.Core.SDK.Interfaces
{
    public interface ISlimePlugin
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string Description { get; }
        void AddNewButton(FlexLayout toolbar);
        // Hooks for application events
        void OnEditorTextChanged(Editor editor, string oldText,
            string newText, Options options);
        void OnEditorCompleted(Editor editor, Options options);

        void OnFileOpened(string filePath, ref string fileContent);

        void OnFileSaving(string filePath, ref string fileContent);
        void AddFileToDocumentParentDirectory(DocumentInfo document,
             string filePath);
    }
}
// #endif