using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Center.MediaBrowser
{
    public class MediaColumn
    {
        public int ColumnIndex { get; set; }
        public List<MediaColumn> MediaColumns { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
    }
}
