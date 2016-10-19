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
        string formFactor;
        bool modular;
        string brand;
        string height;
        string depth;
        string width;
        string weight;

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "effect":
                        power = info.Value;
                        break;
                    case "formfaktor":
                        formFactor = info.Value;
                        break;
                    case "modularitet":
                        modular = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "mærke":
                        brand = info.Value;
                        break;
                    case "vægt":
                        weight = info.Value;
                        break;
                    case "højde":
                        height = info.Value;
                        break;
                    case "bredde":
                        depth = info.Value;
                        break;
                    case "bybde":
                        width = info.Value;
                        break;
                }
            }
        }
    }
}
