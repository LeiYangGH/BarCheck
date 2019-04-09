using BarCheck.Properties;
using BarCheck.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace BarCheck.Views
{
    /// <summary>
    /// ValidateRulesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ValidateRulesWindow : Window
    {
        private string oldSelectedT2VRuleName;
        public ValidateRulesWindow()
        {
            InitializeComponent();
            this.oldSelectedT2VRuleName = Settings.Default.SelectedT2VRuleName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ValidateRulesViewModel setVRVM = (this.DataContext) as ValidateRulesViewModel;
            if (this.oldSelectedT2VRuleName != setVRVM.SelectedT2VRuleName)
            {
                BarcodeHistory.Instance.Close();
                BarcodeHistory.Instance.DeleteIfEmpty();
                Settings.Default.SelectedT2VRuleName = setVRVM.SelectedT2VRuleName;
                Settings.Default.Save();
                Log.Instance.Logger.Info($"SettingWindow btnOK_Click saved SelectedT2VRuleName={setVRVM.SelectedT2VRuleName}");
                Messenger.Default.Send<string>(setVRVM.SelectedT2VRuleName, nameof(MainViewModel));
                Messenger.Default.Send<string>(setVRVM.SelectedT2VRuleName, nameof(BarcodeHistory));
            }
            this.DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ValidateRulesViewModel setVRVM = (this.DataContext) as ValidateRulesViewModel;
            setVRVM.AutoSelectLastValidateRule(Settings.Default.SelectedT2VRuleName);
        }
    }
}
