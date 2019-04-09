using BarCheck.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace BarCheck.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainVM;
        public MainWindow()
        {
            InitializeComponent();
            this.mainVM = this.DataContext as MainViewModel;
            this.ShowVersion();
            Log.Instance.Logger.Info("\r\nUI started!");

        }
        public void ScrollListBoxToButtom(AllBarcodeViewModel allVM)
        {
            this.lstAll.ScrollIntoView(allVM);
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

            "AAA0000000X",
            "15184432110",
            "ABCDE32110",
            "AXGDEGj2110",
            "A0000000X",
            "A00000000000X",
            "A00000000000Y",
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
            if (e.Key == Key.A && this.mainVM != null)
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
            BarcodeHistory.Instance.Close();
            BarcodeHistory.Instance.DeleteIfEmpty();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if NOLOGIN
#else
            LoginWindow win = new LoginWindow();
            win.Owner = this;
            if (win.ShowDialog() ?? true)
            {
                ValidateRulesWindow vrWin = new ValidateRulesWindow();
                vrWin.DataContext = new ValidateRulesViewModel();
                vrWin.ShowDialog();
                //this.mainVM.LoadLastHistory();
            }
            else
            {
                Log.Instance.Logger.Info("Login failed! Will exit application!");
                Application.Current.Shutdown();
            }
#endif
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

#if HELP
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Constants.HelpPdf))
            {
                try
                {
                    Process.Start(Path.Combine(Environment.CurrentDirectory, "User_Guide.pdf"));
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show($"没找到帮助文档{Constants.HelpPdf}");
        }
#endif
    }
}
