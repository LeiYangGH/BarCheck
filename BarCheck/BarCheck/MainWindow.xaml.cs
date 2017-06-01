using BarCheck.ViewModel;
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

namespace BarCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainVM;
        public static MainWindow Instance;
        public MainWindow()
        {
            InitializeComponent();
            this.mainVM = this.DataContext as MainViewModel;
            MainWindow.Instance = this;
            this.Closing += (s, e) =>
              {
                  BarcodeHistory.Instance.Close();
              };
        }
    }
}
