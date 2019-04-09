using BarCheck.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BarcodeFormatsValidator
{
    class Program
    {

        static Dictionary<string, List<ValidateRule>> ReadBarcodeFormatsXml(string fileName)
        {
            var dict =
                new Dictionary<string, List<ValidateRule>>();
            XElement rules = XElement.Load(fileName);
            foreach (var t1 in rules.Elements())
            {
                var t2s = new List<ValidateRule>();
                foreach (var t2 in t1.Elements())
                {
                    var rule = new ValidateRule(t2.Name.LocalName, t2.Value);
                    t2s.Add(rule);
                }
                dict.Add(t1.Name.LocalName, t2s);
            }
            return dict;
        }

        static IEnumerable<string> ReadAllBarcodes(string fileName)
        {
            return File.ReadAllLines(fileName).Select(line => line.Trim()).Distinct();
        }

        static Dictionary<string, List<ValidateRule>> dictRules;
        static string inputFile = @"AllBarcodes.txt";
        static string formatFile = @"BarcodeFormats.xml";
        static string validateResultFile = @"ValidateResult.txt";

        static void ValidateBarcode(string barcode,
              Dictionary<string, Regex> dictReg,
            StreamWriter sw)
        {
            var matchedNames = dictReg.Where(kv => kv.Value.IsMatch(barcode))
                .Select(kv => kv.Key);
            if (matchedNames == null || matchedNames.Count() == 0)
            {
                string msg = $"{barcode}无匹配！";
                sw.WriteLine(msg);
                Console.WriteLine(msg);
            }
            else if (matchedNames.Count() == 1)
            {
                string msg = $"{barcode}<------------------------->{matchedNames.First()}。";
                sw.WriteLine(msg);
                Console.WriteLine(msg);
            }
            else
            {
                string msg = $"{barcode}匹配了多个:{string.Join(" ", matchedNames.ToArray())}";
                sw.WriteLine(msg);
                Console.WriteLine(msg);
            }
        }

        static void Main(string[] args)
        {
            if (!File.Exists(formatFile))
            {
                Console.WriteLine($"请在exe旁边放{formatFile}");
                Console.ReadKey();
                return;
            }
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"请在exe旁边放{inputFile}，内含每行一个条码，空格忽略");
                Console.ReadKey();
                return;
            }
            dictRules = ReadBarcodeFormatsXml(formatFile);
            Dictionary<string, Regex> dictReg = new Dictionary<string, Regex>();
            foreach (ValidateRule vr in dictRules.SelectMany(d => d.Value).Distinct())
            {
                //dictReg.Add(vr.Name,new Regex($"^{vr.RegStr.Trim()}$", RegexOptions.Compiled));
                dictReg.Add(vr.Name, new Regex($"{vr.RegStr.Trim()}", RegexOptions.Compiled));
            }

            using (StreamWriter sw = new StreamWriter(validateResultFile))
            {

                foreach (string barcode in ReadAllBarcodes(inputFile))
                {
                    ValidateBarcode(barcode, dictReg, sw);
                }
            }
            Console.WriteLine($"结果已输出到{validateResultFile}。");
            Console.ReadKey();
        }
    }
}
