using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperGallery.Shared.DBModels
{
    public class MediaFile
    {
        public MediaFile(string source, bool isVideo, DateTime bestGuess, string bestGuessSource)
        {
            SourcePath = source;
            Kind = isVideo ? "video" : "photo";

            if (bestGuess < new DateTime(2003, 1, 1) || bestGuess > DateTime.Now)
                throw new ArgumentException($"Unplausible best guess: {bestGuess}");

            BestGuess = bestGuess;
            BestGuessYear = (short)bestGuess.Year;
            BestGuessMonth = (byte)bestGuess.Month;

            ThumbGuid = Guid.NewGuid().ToString();
            BestGuessSource = bestGuessSource;
        }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public MediaFile()
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        {
        }

        public int Id { get; set; }
        public string ThumbGuid { get; set; }
        public string SourcePath { get; set; }
        public string Kind { get; set; }
        public DateTime BestGuess { get; set; }
        public short BestGuessYear { get; set; }
        public byte BestGuessMonth { get; set; }
        public string BestGuessSource { get; set; }
        public string LocalMediaPath { get; set; }
        public string BestGuessMimeType { get; set; }

        public string GetThumbnailFilename()
        {
            return $"{BestGuess.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}_{ThumbGuid}.jpg";
        }

        public string GetMediaFilename_MP4()
        {
            return $"{BestGuess.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}_{ThumbGuid}.mp4";
        }

        public string GetMediaFilename_Photo()
        {
            return $"{BestGuess.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}_{ThumbGuid}.jpg";
        }
    }
}
