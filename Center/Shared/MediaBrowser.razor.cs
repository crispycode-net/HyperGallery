using Center.MediaBrowser;

namespace Center.Shared
{
    public partial class MediaBrowser
    {
        public enum NavigationModes { Year, Month, MediaItem }
        public NavigationModes NavigationMode { get; set; } = NavigationModes.MediaItem;

        public List<MediaColumn> MediaColumns { get; set; } = new List<MediaColumn>();
    }    
}