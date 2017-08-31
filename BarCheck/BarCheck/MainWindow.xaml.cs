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
            this.ShowVersion();
            Log.Instance.Logger.Info("\r\nUI started!");

        }

        private void ShowVersion()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();
            this.Title += " -" + version;
        }
#if A
        int currentIndex = 0;
        List<string> lstBarcodes = new List<string>()
        {
            "B000000000001",
            "NR",
            "NR",
            "NR",
            "NR",
            "NR",
            "NR",
            "B000000000003",
            "B000000000003",
            "NR",
            "NR",
            "NR",
            "B000000000004",
            "B000000000005",
            "B000000000005",
            "B000000000005",
            "B000000000004",
            "B000000000006",
            "B000000000006",
            "Z111111111111"
        };
#endif

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
#if A
            if (e.Key == Key.A)
            {

                if (this.currentIndex < this.lstBarcodes.Count)
                    this.mainVM.GotBarcode(this.lstBarcodes[this.currentIndex++]);
                else
                    this.mainVM.AddRandomBarcode();
            }
#endif
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
#if NOLOGIN
            return;
#else
            if (this.mainVM.ObsAllBarcodes.Count > 0)
            {
                if (MessageBox.Show("真的要关闭吗?显示的数据会丢失", "关闭程序", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Log.Instance.Logger.Info("UI closed!\r\n\r\n");
                }
                else
                    e.Cancel = true;
            }
#endif
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if NOLOGIN
            return;
#else
            LoginWindow win = new LoginWindow();
            win.Owner = this;
            if (!(win.ShowDialog() ?? true))
            {
                this.Close();
            }
#endif
        }
    }
}
