using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class HardDrive : ComputerComponents
    {
        public bool isInternal;
        public string type = "";
        public string formFactor = "";
        public string capacity = "";
        public string cacheSize = "";
        public string transferRate = "";
        public string brand = "";
        public string sata = "";
        public string height = "";
        public string depth = "";
        public string width = "";

        HardDrive(int id, string category) : base(id, category)
        {
        }
    }
}
