using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class HardDrive : ComputerComponents
    {
        bool isInternal;
        string type;
        string formFactor;
        string capacity;
        string cacheSize;
        string transferRate;
        string brand;
        string sata;
        string height;
        string depth;
        string width;

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "intern/ extern":
                        isInternal = (info.Value.ToLower() == "intern") ? true : false;
                        break;
                    case "type":
                        type = info.Value;
                        break;
                    case "formfactor":
                        formFactor = info.Value;
                        break;
                    case "harddisk størrelse":
                        capacity = info.Value;
                        break;
                    case "cachehukommelse":
                        cacheSize = info.Value;
                        break;
                    case "tranfer rate":
                        transferRate = info.Value;
                        break;
                    case "mærke":
                        brand = info.Value;
                        break;
                    case "sata":
                        sata = info.Value;
                        break;
                    case "højde":
                        height = info.Value;
                        break;
                    case "bybde":
                        depth = info.Value;
                        break;
                    case "bredde":
                        width = info.Value;
                        break;
                }
            }
        }
    }
}
