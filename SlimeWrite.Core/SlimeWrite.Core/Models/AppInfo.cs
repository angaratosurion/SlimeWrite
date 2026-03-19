using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeWrite.Core.Models
{
    public class AppInfo
    {
        public string AppName { get; set; }
        public string Version { get; set; }
        public string Website { get; set; } = "https://github.com/angaratosurion/SlimeWrite";
        public string Copyright { get; set; }
        public string Description { get; set; }
    }
}
