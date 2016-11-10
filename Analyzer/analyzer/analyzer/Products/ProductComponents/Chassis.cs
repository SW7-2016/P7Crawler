using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class Chassis : ComputerComponents
    {
        public string type = "";
        public string fans = "";
        public string brand = "";
        public string weight = "";
        public string height = "";
        public string depth = "";
        public string width = "";
        public bool atx;
        public bool miniAtx;
        public bool miniItx;
    }
}
