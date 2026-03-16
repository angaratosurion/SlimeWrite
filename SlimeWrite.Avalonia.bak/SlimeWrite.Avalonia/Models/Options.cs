using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeWrite.Avalonia.Models
{
    public class Options
    {
        public bool UseTextChangedEvent { get; set; }
        public bool UseEnterPressed { get; set; }
        public int WebViewOrientation { get; set; }
        public Boolean AutoUpdateUsingGithub { get; set; }
    }
}
