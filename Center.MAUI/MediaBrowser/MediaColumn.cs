using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Center.MAUI.MediaBrowser
{
    public class MediaColumn
    {
        public int? DataColumnIndex { get; set; }
        public int? GridColumnIndex { get; set; }
        public bool IsFirstVisibleColumn { get; set; }
        public List<MediaColumn> MediaColumns { get; set; }
        public int Year { get; set; }
        public byte Month { get; set; }
        public string MonthName { get; set; }

        public MediaItem ItemInRow_1 { get; set; }
        public MediaItem ItemInRow_2 { get; set; }

        public string DisplayYear => IsFirstVisibleColumn ? Year.ToString() : "";
        public string DisplayMonth => IsFirstVisibleColumn ? Month.ToString() : "";
    }

    public class MediaItem
    {
        public string ShortName { get; set; }
        public string ThumnailImage { get; set; }
    }
}
