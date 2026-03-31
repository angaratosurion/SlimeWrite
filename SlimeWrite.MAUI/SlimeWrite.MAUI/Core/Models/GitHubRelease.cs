namespace SlimeWrite.MAUI.Core.Models
{
    public class GitHubRelease
    {
        public string tag_name { get; set; }
        public Asset[] assets { get; set; }
    }
}
