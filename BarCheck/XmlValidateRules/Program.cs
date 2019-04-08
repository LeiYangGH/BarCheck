using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace XmlValidateRules
{
    class Program
    {
        static string fileName = @"C:\Users\LeiYang\Downloads\t\BarcodeFormats.xml";
        static void CreateSampleXml(string fileName)
        {
            XElement Rules =
            new XElement("Rules",
                new XElement("厂家一",
                    new XElement("组件一1", "[ABC]{3}[0-9]{5,9}X"),
                    new XElement("组件一2", "[A-Z]{5,}")),
                new XElement("厂家二",
                    new XElement("组件二1", "[0-9]{{5,9}X"),
                    new XElement("组件二2", "[A-Z]{5,}[0-9]"))
            );
            Rules.Save(fileName);
        }

        static Dictionary<string, List<Tuple<string, string>>> ReadXmlRules(string fileName)
        {
            var dict =
                new Dictionary<string, List<Tuple<string, string>>>();

            XElement rules = XElement.Load(fileName);
            foreach (var t1 in rules.Elements())
            {
                var t2s = new List<Tuple<string, string>>();
                foreach (var t2 in t1.Elements())
                {
                    var tu = Tuple.Create(t2.Name.LocalName, t2.Value);
                    t2s.Add(tu);

                }
                dict.Add(t1.Name.LocalName, t2s);
            }
            return dict;

        }

        static void ShowDict(Dictionary<string, List<Tuple<string, string>>> dict)
        {
            foreach (var kv in dict)
            {
                Console.WriteLine(kv.Key);
                foreach (var tu in kv.Value)
                    Console.WriteLine($"{tu.Item1}\t{tu.Item2}");
            }

        }

        static void Main(string[] args)
        {
            //CreateSampleXml(fileName);
            Dictionary<string, List<Tuple<string, string>>> dict = ReadXmlRules(fileName);
            ShowDict(dict);
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
