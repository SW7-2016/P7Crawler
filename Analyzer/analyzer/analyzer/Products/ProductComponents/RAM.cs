using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class RAM : ComputerComponents
    {
        public string type = "";
        public string capacity = "";
        public string speed = "";
        public string technology = "";
        public string formFactor = "";
        public string casLatency = "";

        RAM(int id, string category) : base(id, category)
        {
        }
    }
}
