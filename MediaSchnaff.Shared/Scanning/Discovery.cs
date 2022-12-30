using MediaSchnaff.Shared.DBAccess;
using MediaSchnaff.Shared.DBModels;
using MediaSchnaff.Shared.LocalData;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MediaSchnaff.Shared.Scanning
{
    public interface IDiscovery
    {
        void Scan(string startDirectory, bool scanRecursively, CancellationToken ct);
    }

    public class Discovery : IDiscovery
    {
        private readonly ILogger<Discovery> logger;
        private readonly MainContext mainContext;
        private readonly IDirectories directories;
        private readonly HashSet<string> ignoreExtensions;
        public static DateTime PlausibMinDate = new DateTime(2003, 1, 1);

        /// <summary>
        /// Optimal thumb size is 640 x 430 for UHD @150% scaling
        /// UHD @ 100% = 3840 x 2160
        /// UHD @ 150% = 2560 x 1440
        /// Abzüglich dem oberend Rand von 150 Pixel bleiben für die Thumbs 2560 x 1290
        /// Bei 3 Reihen und 4 Spalten hat jedes Thumbnail einen Rahmen von 640 x 430
        /// </summary>
        public static Size FixedThumbnailFrameSize = new Size(640, 430);
        private readonly string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "Where am I?";

        public Discovery(ILogger<Discovery> logger, MainContext mainContext, IDirectories directories)
        {
            this.logger = logger;
            this.mainContext = mainContext;
            this.directories = directories;
            this.ignoreExtensions = new HashSet<string> { 
                ".aae",".dat",
                ".ini",".db",".hps-metadata",".txt",".zip",".thm",".pls",".pdf",
                ".dng", ".raf"
            };
        }

        public void Scan(string startDirectory, bool scanRecursively, CancellationToken ct)
        {
            // Was cancellation already requested?
            if (ct.IsCancellationRequested)
            {
                return;
            }

            try
            {
                logger.LogInformation($"Start scanning in directory {startDirectory}");

                var allFiles = System.IO.Directory.GetFiles(startDirectory, "*.*", scanRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                var allDbFileSourcePaths = mainContext.Files?.Select(f => f.SourcePath).ToDictionary(f => f);
                var allDbFileSourcePathsWithErrors = mainContext.ScanErrors?.Select(f => f.SourcePath).ToDictionary(f => f);

                int index = 0;  

                foreach (var source in allFiles)
                {
                    logger.LogInformation($"Processing file {index} of {allFiles.Length}: {source}");
                    index++;

                    if (allDbFileSourcePaths?.ContainsKey(source) == true)
                        continue;
                    if (allDbFileSourcePathsWithErrors?.ContainsKey(source) == true)
                        continue;

                    var mfFileExt = Path.GetExtension(source).ToLower();
                    if (ignoreExtensions.Contains(mfFileExt))
                        continue;


                    // if (mfFileExt == ".mov" || mfFileExt == ".avi" || mfFileExt == ".mp4" || mfFileExt == ".mpg" || mfFileExt == ".3gp" || mfFileExt == ".m4v")
                    {
                        var newFile = GetFileInfo(source, mfFileExt);
                        if (newFile.Error == null && newFile.File == null)
                            throw new Exception($"Non deterministic! File {source} slipped");


                        if (newFile.File != null)
                        {
                            // Wir habe die meta infos. Wenns jetzt noch ein Thumbnail gibt, ist alles gut
                            var error = CreateThumbnail(newFile.File);
                            if (error == null)
                            {
                                mainContext.Files?.Add(newFile.File);
                            }
                            else
                            {
                                mainContext.ScanErrors?.Add(new ScanError(error, source));
                            }
                        }
                        else if (newFile.Error != null)
                        {
                            mainContext.ScanErrors?.Add(new ScanError(newFile.Error, source));
                        }

                        mainContext.SaveChanges();
                    }
                }
                
                logger.LogInformation($"Total files in directory {startDirectory}: {allFiles.Length}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}");
            }
        }

        private string? CreateThumbnail(MediaFile mf)
        {
            try
            {
                var thumbDir = Path.Combine(directories.ThumbnailDir, mf.BestGuessYear.ToString());
                if (!System.IO.Directory.Exists(thumbDir))
                    System.IO.Directory.CreateDirectory(thumbDir);
                var output = Path.Combine(thumbDir, mf.GetThumbnailFilename());

                var originalImagePath = mf.SourcePath;

                if (mf.Kind == "video")
                {
                    IConversion conversion = FFmpeg.Conversions.FromSnippet.Snapshot(mf.SourcePath, output, TimeSpan.FromSeconds(0)).GetAwaiter().GetResult();
                    IConversionResult result = conversion.Start().GetAwaiter().GetResult();

                    originalImagePath = output;
                }

                IImageFormat format;
                var image = Image.Load(File.ReadAllBytes(originalImagePath), out format);
                image.Mutate(x => x.Resize(CalculateAspectRatioFit(image.Size(), FixedThumbnailFrameSize)));
                image.SaveAsJpeg(output, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}");
                return ex.Message;
            }
        }

        //private static Size SizeToHeight(Size image, int fixedHeight)
        //{
        //    var scaleFactor = (double)fixedHeight / (double)image.Height;            
        //    Size result = new Size((int)(image.Width * scaleFactor), fixedHeight);
        //    return result;
        //}

        private static Size CalculateAspectRatioFit(Size srcSize, Size maxSize)
        {
            var ratio = Math.Min((double)maxSize.Width / (double)srcSize.Width, (double)maxSize.Height / (double)srcSize.Height);
            return new Size((int)(srcSize.Width * ratio), (int)(srcSize.Height * ratio));
        }

        private MediaFileOrError GetFileInfo(string path, string mfFileExt)
        {
            try
            {
                DateTime mfDateCreated = File.GetCreationTime(path);
                DateTime mfDateModified = File.GetLastWriteTime(path);
                DateTime mfMinFileDate = mfDateCreated;
                if (mfDateModified < mfMinFileDate)
                    mfMinFileDate = mfDateModified;

                if (mfFileExt == ".tif")
                {
                    var directories = ImageMetadataReader.ReadMetadata(path);
                    var createdTag = from dir in directories.OfType<ExifIfd0Directory>()
                                     from tag in dir.Tags
                                     where tag.Name == "Date/Time"
                                     select tag;

                    var huh = createdTag.FirstOrDefault()?.Description;
                    if (!string.IsNullOrEmpty(huh))
                    {
                        var mfDateTimeOriginal = DateTime.ParseExact(huh, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                        return new MediaFileOrError(new MediaFile(path, false, mfDateTimeOriginal, "Exif, Tag=Date/Time"), null);
                    }
                    else
                    {
                        return new MediaFileOrError(null, "Can't get original datetime from tif");
                    }
                }
                else if (mfFileExt == ".jpg" || mfFileExt == ".jpeg")
                {
                    //IImageFormat format;
                    //var image = Image.Load(File.ReadAllBytes(path), out format);

                    var image = Image.Identify(path);

                    var exif = image.Metadata.ExifProfile;
                    var eValue = exif?.GetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeOriginal);
                    if (eValue != null)
                    {
                        var mfDateTimeOriginal = DateTime.ParseExact(
                            (string)eValue.Value,
                            "yyyy:MM:dd HH:mm:ss",
                            CultureInfo.InvariantCulture);

                        return new MediaFileOrError(new MediaFile(path, false, mfDateTimeOriginal, "Exif, Tag=DateTimeOriginal"), null);
                    }
                    else
                    {
                        return new MediaFileOrError(new MediaFile(path, false, mfMinFileDate, "Min. File date"), null);
                    }
                }
                else if (IsViedeo(mfFileExt))
                {
                    var mfOriginalFileName = Path.GetFileName(path);
                    if (mfOriginalFileName.StartsWith("WP_20"))
                    {
                        var mfDateTimeOriginal = DateTime.ParseExact(mfOriginalFileName.Substring(3, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
                        return new MediaFileOrError(new MediaFile(path, false, mfDateTimeOriginal, "Filename (WP_20...)"), null);
                    }
                    else
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = Path.Combine(exePath, "ffmpeg.exe");

                        if (!File.Exists(process.StartInfo.FileName))
                        {
                            logger.LogWarning($"FFMpeg not found in {process.StartInfo.FileName}, will try to auto download...");

                            FFmpeg.SetExecutablesPath(exePath);
                            FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official).GetAwaiter().GetResult();

                            logger.LogInformation($"...done");
                        }


                        process.StartInfo.Arguments = $"-i \"{path}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        //* Read the output (or the error)
                        string output = process.StandardOutput.ReadToEnd();
                        string err = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        output = output + err;

                        string[] lines = output.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            if (line.Trim().StartsWith("creation_time"))
                            {
                                string dateString = line.Substring(line.IndexOf(":") + 2);

                                DateTime crTime;
                                bool success = DateTime.TryParseExact(dateString, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out crTime);
                                if (!success)
                                    success = DateTime.TryParse(dateString, null, DateTimeStyles.RoundtripKind, out crTime);

                                if (success && crTime >= PlausibMinDate && crTime <= DateTime.Now)
                                {
                                    var mfDateTimeOriginal = crTime;
                                    return new MediaFileOrError(new MediaFile(path, true, mfDateTimeOriginal, "FFmpeg meta data creation_time"), null);
                                }
                            }
                        }

                        return new MediaFileOrError(new MediaFile(path, true, mfMinFileDate, "Min. File date"), null);
                    }
                }
                else
                {
                    return new MediaFileOrError(null, $"Unknown file extension: {mfFileExt}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message} in file {path}");
                return new MediaFileOrError(null, ex.Message);
            }
        }

        private static bool IsViedeo(string mfFileExt)
        {
            return mfFileExt == ".mov" || mfFileExt == ".avi" || mfFileExt == ".mp4" || mfFileExt == ".mpg" || mfFileExt == ".3gp" || mfFileExt == ".m4v" || mfFileExt == ".mp4";
        }

        private record MediaFileOrError(MediaFile? File, string? Error);
    }
}