using SlimeWrite.Core.Models;
using SlimeWrite.Core.SDK.Interfaces;
using System;

namespace SlimeWrite.Plugins.Standard.AutoCorrectPlugin
{
    // All the code in this file is included in all platforms.
    public class AutoCorrectPlugin : ISlimePlugin
    {
        public string Name => "Auto-Correct and Timestamp Plugin";
        public string Version => "1.0.0";
        public string Author => "Developer";
        public string Description => "Automatically corrects text and adds timestamps.";

        public void AddFileToDocumentParentDirectory(DocumentInfo document, string filePath)
        {
             
        }

        public void AddNewButton(FlexLayout toolbar)
        {
            //toolbar.Add(new Button
            //{
            //    Text = "Auto-Correct",
            //    Command = new Command(() =>
            //    {
            //        // Example action: Show a message or perform an auto-correct action
            //        // For demonstration, we'll just show a message box (if supported)
            //        // In a real plugin, you might trigger an auto-correct function here
            //        Console.WriteLine("Auto-Correct button clicked!");
            //    })
            //});
            //// throw new NotImplementedException();
        }

        public void OnEditorCompleted(Editor editor, Options options)
        {
            editor.Text = editor.Text.Replace("(c)", "©");
        }

        // Called in: editor_TextChanged
        public void OnEditorTextChanged(Editor editor, string oldText, string newText, Options options)
        {
            if (string.IsNullOrEmpty(newText))
                return;

            // Example: if user types (c), convert it to ©
            if (newText.Contains("(c)"))
            {
                // MainPage temporarily disables event, so we can safely modify text
                editor.Text = newText.Replace("(c)", "©");
            }
        }

        // Called in: OpenFile (Segmented or Normal)
        public void OnFileOpened(string filename, ref string fileContent)
        {
            // Modify text right after file is loaded
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                fileContent = "# New Markdown Document\n\nStart writing here...";
            }
        }

        // Called in: SaveFile
        public void OnFileSaving(string filename, ref string textToSave)
        {
            // Add hidden markdown comment with last save timestamp
            if (!string.IsNullOrEmpty(textToSave))
            {
                textToSave += $"\n\n<!-- Last saved on: {DateTime.Now} -->";
            }
        }
    }
}