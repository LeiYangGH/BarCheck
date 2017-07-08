using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestSPWrite
{
    public class SerialPortAccessor
    {
        private static readonly Lazy<SerialPortAccessor> lazy =
            new Lazy<SerialPortAccessor>(() => new SerialPortAccessor());
        public static SerialPortAccessor Instance { get { return lazy.Value; } }

        static SerialPort serialPort = new SerialPort();


        private SerialPortAccessor()
        {
            try
            {
                serialPort.PortName = "COM3";//修改要读的串口名字
                serialPort.BaudRate = 9600;//波特率
                //Parity p = serialPort.Parity;
                //serialPort.Parity = Parity.None;
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void SendBytes(byte[] bytes)
        {
            try
            {
                serialPort.Write(bytes,0,bytes.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //public void SendText(string text)
        //{
        //    try
        //    {
        //        serialPort.Write(text);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}


    }
    }
