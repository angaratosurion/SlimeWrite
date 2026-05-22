
using SlimeWrite.Core.Models;
using SlimeWrite.SDK.Interfaces;

namespace SlimeWrite.Plugins.Standard.AutoCorrectPlugin
{
    // All the code in this file is included in all platforms.
    public class AutoCorrectPlugin : ISlimePlugin
    {
        public string Name => "Auto-Correct and Timestamp Plugin";
        public string Version => "1.0.0";
        public string Author => "Developer";

        // Καλούνταν στο: editor_TextChanged
        public void OnEditorTextChanged(Editor editor, string oldText, string newText, Options options)
        {
            if (string.IsNullOrEmpty(newText)) return;

            // Παράδειγμα: Αν ο χρήστης γράψει (c), το μετατρέπει αυτόματα σε ©
            if (newText.Contains("(c)"))
            {
                // Το MainPage κλείνει προσωρινά το event, οπότε μπορούμε να αλλάξουμε το κείμενο με ασφάλεια
                editor.Text = newText.Replace("(c)", "©");
            }
        }

       

        // Καλούνταν στο: OpenFile (είτε Segmented είτε Normal)
        public void OnFileOpened(string filename, ref string fileContent)
        {
            // Μπορείς να τροποποιήσεις το κείμενο που μόλις διαβάστηκε από το αρχείο
            // Για παράδειγμα, αν το αρχείο είναι άδειο, βάλε έναν έτοιμο τίτλο
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                fileContent = "# Νέο Έγγραφο Markdown\n\nΞεκινήστε να γράφετε εδώ...";
            }
        }

        // Καλούνταν στο: SaveFile
        public void OnFileSaving(string filename, ref string textToSave)
        {
            // Προσθέτει ένα κρυφό σχόλιο Markdown με την ημερομηνία τελευταίας αποθήκευσης στο τέλος του αρχείου
            if (!string.IsNullOrEmpty(textToSave))
            {
                textToSave += $"\n\n<!-- Last saved on: {DateTime.Now} -->";
            }
        }
    }
}
