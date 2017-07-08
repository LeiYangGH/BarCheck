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

namespace TestSPWrite
{
    /// <summary>
    /// UCBytes.xaml 的交互逻辑
    /// </summary>
    public partial class UCBytes : UserControl
    {
        public UCBytes()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string BytesText
        {
            get { return (string)GetValue(BytesTextProperty); }
            set { SetValue(BytesTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Property1.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BytesTextProperty
            = DependencyProperty.Register(
                  "BytesText",
                  typeof(string),
                  typeof(UCBytes),
                  new PropertyMetadata("")
              );

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(this.BytesText);
            btn.IsEnabled = false;
            byte[] bytes = StringToByteArray(this.BytesText);
            SerialPortAccessor.Instance.SendBytes(bytes);

            btn.IsEnabled = true;
        }

        private byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
