namespace SlimeWrite.MAUI.Core.Models
{
    public class Options
    {
        public bool UseTextChangedEvent { get; set; }
        public bool UseEnterPressed { get; set; }
        public int WebViewOrientation { get; set; }
        public bool AutoUpdateUsingGithub { get; set; }
        public bool SegmentedLoading { get; set; }
        public int MaxSegmentLength { get; set; } 
    }
}
