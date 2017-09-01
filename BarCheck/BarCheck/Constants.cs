using BarCheck.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BarCheck
{
    public static class Constants
    {
        public const char GradeSplitChar = '+';
        public const string GradeSplitString = "+";
        public const string GradeFailScanString = "-";
        public const string NR = "NR";
        public const string GradeYES = "合格";
        public const string GradeNO = "不合格";
        public const string PortOpen = "扫描串口已打开";
        public const string PortClosed = "扫描串口已关闭";
        public const string Dup = "重复";
        public const string sndOK = @"Wav\OK.wav";
        public const string sndNR = @"Wav\NR.wav";
        public const string sndDup = @"Wav\DUP.wav";
        public static readonly Dictionary<BarcodeStatus, string> dicStatusDesc
            = new Dictionary<BarcodeStatus, string>()
            {
                { BarcodeStatus.NO,GradeNO },
                { BarcodeStatus.Dup,Dup },
                { BarcodeStatus.Yes,GradeYES },
            };
        public static readonly Dictionary<BarcodeStatus, SolidColorBrush> dicStatusBrush
            = new Dictionary<BarcodeStatus, SolidColorBrush>()
            {
                        { BarcodeStatus.NO,Brushes.Red },
                        { BarcodeStatus.Dup,Brushes.Orange },
                        { BarcodeStatus.Yes,Brushes.Green },
            };
        public const string ComUnknown = "COM?";
        public const string Users = "users";
        public static readonly string HelpPdf = Path.Combine(Path.Combine(Environment.CurrentDirectory, "User_Guide.pdf"));

        public static readonly byte[] LightAllOn = { 0x55, 0x01, 0x02, 0x03, 0x04, 0xEE };
        public static readonly byte[] LightAllOff = { 0x55, 0xF1, 0xF2, 0xF3, 0xF4, 0xEE };
        public static readonly byte[] LightOK = { 0x55, 0x01, 0xF2, 0x03, 0xF4, 0xEE };
        public static readonly byte[] LightDup = { 0x55, 0x01, 0x02, 0xF3, 0x04, 0xEE };
        public static readonly byte[] LightNR = { 0x55, 0xF1, 0x02, 0xF3, 0x04, 0xEE };
    }
}
