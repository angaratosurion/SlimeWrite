using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeWrite.Avalonia.Models
{
    public class GitHubRelease
    {
        public string tag_name { get; set; }
        public Asset[] assets { get; set; }
    }
}
