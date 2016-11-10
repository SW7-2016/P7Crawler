using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class PSU : ComputerComponents
    {
        public string power = "";
        public string formFactor = "";
        public string brand = "";
        public string height = "";
        public string depth = "";
        public string width = "";
        public string weight = "";
        public bool modular;

    }
}
