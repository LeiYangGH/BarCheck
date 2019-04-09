using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarCheck.Data
{
    public class ValidateRule
    {
        public ValidateRule(string name, string regStr)
        {
            this.Name = name;
            this.RegStr = regStr;
        }

        public string Name { get; set; }
        public string RegStr { get; set; }
    }
}
