using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaSchnaff.Center
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 2010; i < 2021; i++)
                {
                    YearViewModel item = new YearViewModel(i);
                    for (int t = 0; t < 98; t++)
                    {
                        item.Thumbs.Add(new ThumbViewModel(t.ToString(), "..."));
                    }
                    ViewModel.Years.Add(item);
                }
            }
            catch (Exception ex)
            {
                ViewModel.ErrorMessage = ex.Message;
            }
        }

        public CenterViewModel ViewModel => (CenterViewModel)DataContext;
    }
}
