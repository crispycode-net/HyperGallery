using Microsoft.Extensions.Options;

namespace MediaSchnaff.Shared.LocalData
{
    public interface IDirectories
    {
        string StorageDir { get; }
        string LogFile {  get; }
        string ThumbnailDir { get; }
        string MediaDirAbs { get; }
        string WWWRootAbs { get; }
    }

    public class Directories : IDirectories
    {
        private IOptions<ApplicationSettings> Settings;

        public Directories(IOptions<ApplicationSettings> settings)
        {
            this.Settings = settings;
            if (Settings?.Value?.ThumbnailDir == null)
                throw new Exception("No ThumbnailDir configured in application settings");
            if (Settings?.Value?.WWWRoot == null)
                throw new Exception("No WWWRoot configured in application settings");
            if (Settings?.Value?.MediaDirRel == null)
                throw new Exception("No MediaDirRel configured in application settings");
        }

        public string StorageDir
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "MediaSchnaff");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string ThumbnailDir
        {
            get
            {
                var dir = Settings.Value.ThumbnailDir!;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string WWWRootAbs
        {
            get
            {
                var dir = Settings.Value.WWWRoot!;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string MediaDirAbs
        {
            get
            {
                var dir = Path.Combine(Settings.Value.WWWRoot!, Settings.Value.MediaDirRel !);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string LogDir
        {
            get
            {
                var dir = Path.Combine(
                    StorageDir,
                    "Log");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }

        public string LogFile
        {
            get
            {
                return Path.Combine(LogDir, "Log.txt");
            }
        }
    }
}
