using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class SoundCard : ComputerComponents
    {
        string type;
        string speakerSupport;
        string Socket;
        string fullDuplex;

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "type":
                        type = info.Value;
                        break;
                    case "højtalersuppport":
                        speakerSupport = info.Value;
                        break;
                    case "brugerflade":
                        Socket = info.Value;
                        break;
                    case "full duplex":
                        fullDuplex = info.Value;
                        break;
                }
            }
        }
    }
}
