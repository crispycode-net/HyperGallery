using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaSchnaff.Center
{
    public class CenterViewModel : MvxViewModel
    {
        public CenterViewModel()
        {
            Years = new ObservableCollection<YearViewModel>();
            errorMessage = null; 
        }

        public ObservableCollection<YearViewModel> Years { get; set; }

        private string? errorMessage;
        public string? ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                SetProperty(ref errorMessage, value);
                RaisePropertyChanged("ErrorMessageVisible");
            }
        }

        public bool ErrorMessageVisible => errorMessage != null;
    }

    public class YearViewModel : MvxViewModel
    {
        public YearViewModel(int year)
        {
            Year = year;
            Thumbs = new List<ThumbViewModel>();
        }

        private int year;
        public int Year
        {
            get { return year; }
            set { SetProperty(ref year, value); }
        }


        public List<ThumbViewModel> Thumbs { get; set; }
    }

    public class ThumbViewModel
    {
        public ThumbViewModel(string name, string thumbPath)
        {
            Name = name;
            ThumbPath = thumbPath;
        }

        public string Name { get; set; }
        public string ThumbPath { get; set; }
    }
}
