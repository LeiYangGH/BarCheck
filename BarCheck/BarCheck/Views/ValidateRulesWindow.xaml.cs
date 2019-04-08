using BarCheck.Properties;
using BarCheck.ViewModel;
using GalaSoft.MvvmLight.Messaging;
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
using System.Windows.Shapes;

namespace BarCheck.Views
{
    /// <summary>
    /// ValidateRulesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ValidateRulesWindow : Window
    {
        public ValidateRulesWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ValidateRulesViewModel setVRVM = (this.DataContext) as ValidateRulesViewModel;
            Settings.Default.SelectedT2VRuleName = setVRVM.SelectedT2VRuleName;
            Settings.Default.Save();
            Log.Instance.Logger.Info($"SettingWindow btnOK_Click saved SelectedT2VRuleName={setVRVM.SelectedT2VRuleName}");
            Messenger.Default.Send<string>(setVRVM.SelectedT2VRuleName);
            this.DialogResult = true;
        }
    }
}
