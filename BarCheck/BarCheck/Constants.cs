using BarCheck.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public const string sndNR = @"Wav\NR.wav";
        public const string sndDup = @"Wav\DUP.wav";
        public static readonly Dictionary<BarcodeStatus, string> dicStatusDesc
            = new Dictionary<BarcodeStatus, string>()
            {
                { BarcodeStatus.NO,GradeNO },
                { BarcodeStatus.Dup,Dup },
                { BarcodeStatus.Yes,GradeYES },
            };
        public const string ComUnknown = "COM?";
        public const string Users = "users";
        //0110001A000101CE18 声音1+闪光
        //0110001A0001040E1B 声音2+闪光
        //0110001A0001028E19 只有闪光
        //0110001A0001034FD9 只有声音1
        //0110001A000105CFDB 只有声音2
        //0110001A0001000FD8 全关闭
        //0110001A000101CE18  声音1+闪光
        public static readonly byte[] LightAllOn = { 0x55, 0xF1, 0xF2, 0xF3, 0xF4, 0xEE };
        public static readonly byte[] LightAllOff = { 0x55, 0x01, 0x02, 0x03, 0x04, 0xEE };
        public static readonly byte[] LightOK = { 0x55, 0x01, 0x02, 0xF3, 0xF4, 0xEE };
        public static readonly byte[] LightDup = { 0x55, 0xF1, 0x02, 0x03, 0x04, 0xEE };
        public static readonly byte[] LightNR = { 0x55, 0xF1, 0xF2, 0x03, 0x04, 0xEE };


        //01 10 00 1A 00 01 00 0F D8 关闭
    }
}
