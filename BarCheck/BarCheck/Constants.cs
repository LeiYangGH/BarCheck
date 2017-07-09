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
        public const string Dup = "重复";
        //0110001A000101CE18  声音1+闪光
        public static readonly byte[] Alarm1LightBytes = { 0x01,0x10,0x00,0x1A,0x00,0x01,0x01,0xCE,0x18 };
        //01 10 00 1A 00 01 04 0E 1B  声音2+闪光
        public static readonly byte[] Alarm2LightBytes = { 0x01,0x10,0x00,0x1A,0x00,0x01,0x04,0xCE,0x1B };
        //01 10 00 1A 00 01 00 0F D8
        public static readonly byte[] AlarmClose = { 0x01,0x10,0x00,0x1A,0x00,0x01,0x00,0x0F,0xD8 };
    }
}
