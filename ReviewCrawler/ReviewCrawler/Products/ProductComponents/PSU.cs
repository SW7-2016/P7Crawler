using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class PSU : ComputerComponents
    {
        string power;
        string format;
        bool modular;
        string brand;
        string height;
        string depth;
        string width;
        string weight;

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {

        }
    }
}
