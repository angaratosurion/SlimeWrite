using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeWrite.Models
{
    public class DropdownItem
    {
        public string Text { get; set; }
        public string IconSource { get; set; } // Όνομα αρχείου ή URL
        public object Id { get; set; } // Για να ξέρεις τι επιλέχθηκε
    }
}
