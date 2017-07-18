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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                this.mainVM.AddRandomBarcode();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.mainVM.ObsAllBarcodes.Count > 0)
            {
                if (MessageBox.Show("真的要关闭吗?显示的数据会丢失", "关闭程序", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Log.Instance.Logger.Info("UI closed!\r\n\r\n");
                }
                else
                    e.Cancel = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginWindow win = new LoginWindow();
            win.Owner = this;
            if (!(win.ShowDialog() ?? true))
            {
                this.Close();
            }
        }
    }
}
