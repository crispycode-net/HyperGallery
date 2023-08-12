using HeyRed.Mime;
using HyperGallery.Shared.DBAccess;
using HyperGallery.Shared.DBModels;
using HyperGallery.Shared.LocalData;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using static HyperGallery.Shared.Scanning.Discovery;

namespace HyperGallery.Shared.Scanning
{
    public interface IDiscovery
    {
        MediaFileOrError GetFileInfo(string path);
        void Scan(string startDirectory, bool scanRecursively, CancellationToken ct);
        string TranscodeToH264Mp4(string sourceFile, string targetFile);
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

        public static Size FixedMediaFrameSize = new Size(2560, 1440);

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

                    var newFile = GetFileInfo(source, mfFileExt);
                    if (newFile.Error == null && newFile.File == null)
                        throw new Exception($"Non deterministic! File {source} slipped");


                    if (newFile.File != null)
                    {
                        // Wir haben die meta infos. Wenns jetzt noch ein Thumbnail gibt, ist alles gut
                        var error = CreateThumbnailAndMedia(newFile.File);
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
                
                logger.LogInformation($"Total files in directory {startDirectory}: {allFiles.Length}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}");
            }
        }

        private string? CreateThumbnailAndMedia(MediaFile mf)
        {
            try
            {
                var thumbDir = Path.Combine(directories.ThumbnailDirAbs, mf.BestGuessYear.ToString());
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

                //
                // Vom Original wird eine Kopie im Media Verzeichnis abgelegt
                // Bei Videos geht es nicht ohne diesen Schritt, da der Browser viele Formate nicht unterstützt
                // Also transcodierung nach H.264/mp4
                // Bei Bilder nutzen wir den Umstand um eine auf UHD@150% = 2560x1440 Kopie anzulegen
                //


                var mediaDir = Path.Combine(directories.MediaDirAbs, mf.BestGuessYear.ToString());
                if (!System.IO.Directory.Exists(mediaDir))
                    System.IO.Directory.CreateDirectory(mediaDir);

                if (mf.Kind == "video")
                {
                    var target = Path.Combine(mediaDir, mf.GetMediaFilename_MP4());

                    if (Path.GetExtension(mf.SourcePath) == ".mp4")
                    {
                        File.Copy(mf.SourcePath, target, true);
                    }
                    else
                    {                        
                        var error = TranscodeToH264Mp4(mf.SourcePath, target);
                        if (error != null)
                            return error;
                    }

                    mf.LocalMediaPath = target.Replace(directories.WWWRootAbs, "").Trim('\\').Replace("\\", "/");
                    mf.BestGuessMimeType = Discovery.GuessMimeType(target);
                }
                else
                {
                    var target = Path.Combine(mediaDir, mf.GetMediaFilename_Photo());
                    var localImage = Image.Load(File.ReadAllBytes(originalImagePath), out format);
                    localImage.Mutate(x => x.Resize(CalculateAspectRatioFit(localImage.Size(), FixedMediaFrameSize)));
                    localImage.SaveAsJpeg(target, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());

                    mf.LocalMediaPath = target.Replace(directories.WWWRootAbs, "").Trim('\\').Replace("\\", "/");
                    mf.BestGuessMimeType = Discovery.GuessMimeType(target);
                }
                

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}");
                return ex.Message;
            }
        }

        public string TranscodeToH264Mp4(string sourceFile, string targetFile)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = Path.Combine(exePath, "ffmpeg.exe");

                if (!File.Exists(process.StartInfo.FileName))
                {
                    logger.LogWarning($"FFMpeg not found in {process.StartInfo.FileName}, will try to auto download...");

                    FFmpeg.SetExecutablesPath(exePath);
                    FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official).GetAwaiter().GetResult();

                    logger.LogInformation($"...done");
                }

                process.StartInfo.Arguments = $"-i \"{sourceFile}\" -y -vcodec libx264 -f mp4 \"{targetFile}\"";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) => {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    if (outputWaitHandle.WaitOne() && errorWaitHandle.WaitOne())
                    {
                        // Process completed. Check process.ExitCode here.
                        if (process.ExitCode != 0)
                            return $"Fehler beim transcodieren: (timeout)";
                    }
                    else
                    {
                        return $"Fehler beim transcodieren: {error})";
                    }
                }
            }

            return null;
        }

        private static Size CalculateAspectRatioFit(Size srcSize, Size maxSize)
        {
            var ratio = Math.Min((double)maxSize.Width / (double)srcSize.Width, (double)maxSize.Height / (double)srcSize.Height);
            if (ratio > 1) ratio= 1;
            return new Size((int)(srcSize.Width * ratio), (int)(srcSize.Height * ratio));
        }

        public MediaFileOrError GetFileInfo(string path)
        {
            var mfFileExt = Path.GetExtension(path).ToLower();
            return GetFileInfo(path, mfFileExt);
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

                        var dates = GetAllDatesFromFFMpegInfo(output);
                        if (dates.Any())
                        {
                            var oldest = dates.Min(d => d);
                            return new MediaFileOrError(new MediaFile(path, true, oldest, "FFmpeg meta data"), null);
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

        private List<DateTime> GetAllDatesFromFFMpegInfo(string output)
        {
            var zuluDates = Regex.Matches(output, @"[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]*Z");
            var offsetDates = Regex.Matches(output, @"[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}[+][0-9]*");

            List<DateTime> dates = new List<DateTime>();
            foreach (Match zuluMatch in zuluDates)
            {
                if (DateTime.TryParse(zuluMatch.Value, out DateTime parsedDate))
                {
                    if (parsedDate >= PlausibMinDate && parsedDate <= DateTime.Now)
                        dates.Add(parsedDate);
                }
                else
                    throw new Exception("Can't parse date from FFMpeg output: " + zuluMatch.Value);
            }
            foreach (Match offsetMatch in offsetDates)
            {
                if (DateTime.TryParse(offsetMatch.Value, out DateTime parsedDate))
                {
                    if (parsedDate >= PlausibMinDate && parsedDate <= DateTime.Now)
                        dates.Add(parsedDate);
                }
                else
                    throw new Exception("Can't parse date from FFMpeg output: " + offsetMatch.Value);
            }

            return dates;
        }

        public static string GuessMimeType(string file)
        {
            return MimeGuesser.GuessMimeType(file);
        }

        private static bool IsViedeo(string mfFileExt)
        {
            return mfFileExt == ".mov" || mfFileExt == ".avi" || mfFileExt == ".mp4" || mfFileExt == ".mpg" || mfFileExt == ".3gp" || mfFileExt == ".m4v" || mfFileExt == ".mp4";
        }

        public record MediaFileOrError(MediaFile? File, string? Error);
    }
}