using BarCheck.Properties;
using BarCheck.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using UsersFactory;
namespace BarCheck.Views
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private string usersFileName = Path.Combine(Environment.CurrentDirectory, Constants.Users);
        private UsersReadWriter usersRW;
        private List<User> lstUsers;
        public LoginWindow()
        {
            InitializeComponent();
            this.usersRW = new UsersReadWriter(usersFileName);
            this.lstUsers = this.usersRW.ReadFile();
            this.txtU.Text = Settings.Default.CurrentUser;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string name = this.txtU.Text.Trim();
            string pwd = this.txtP.Text;
            string pwdh = User.GetPasswordHash(pwd);
            User u = this.lstUsers.FirstOrDefault(x =>
               x.Name == name
               && x.PasswordHash == pwdh);
            if (u != null)
            {
                MainViewModel.currentUserName = u.Name;
                Settings.Default.CurrentUser = u.Name;
                Settings.Default.Save();
                Log.Instance.Logger.Info($"{name} login succeed");
                this.DialogResult = true;
            }
            else
            {
                Log.Instance.Logger.Info($"{name} login failed");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
