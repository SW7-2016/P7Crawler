using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class Chassis : ComputerComponents
    {
        string type;
        bool atx;
        bool miniAtx;
        bool miniItx;
        string fans;
        string brand;
        string weight;
        string height;
        string depth;
        string width;

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "type":
                        type = info.Value;
                        break;
                    case "atx":
                        atx = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "micro-atx":
                        miniAtx = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "mini-itx":
                        miniItx = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "blæsere":
                        fans = info.Value;
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
