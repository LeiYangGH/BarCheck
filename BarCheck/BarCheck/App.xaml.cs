using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BarCheck
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool isNewInstance = false;
            mutex = new Mutex(true, "BarCheck", out isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("不能同时运行多个程序实例！");
                App.Current.Shutdown();
            }
            else
                base.OnStartup(e);
        }
    }
}
