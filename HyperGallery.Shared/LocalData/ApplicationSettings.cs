namespace HyperGallery.Shared.LocalData
{
    public class ApplicationSettings
    {
        /// <summary>
        /// The base directory. The gallery can only read from sub directories of this
        /// </summary>
        public string? WWWRoot { get; set; }

        /// <summary>
        /// Location where the thumbnails are stored. Relative to WWWRoot
        /// </summary>
        public string? ThumbnailDirRel { get; set; }

        /// <summary>
        /// Location where the medial file copies are stored. Relative to WWWRoot
        /// </summary>
        public string? MediaDirRel { get; set; }

        /// <summary>
        /// A list of directories that will be scanned continously for new content
        /// </summary>
        public List<SourceDir> SourceDirs { get; set; } = new List<SourceDir>();
    }

    public class SourceDir
    {
        public string? Path { get; set; }
        public bool ScanRecursively { get; set; }
    }
}
