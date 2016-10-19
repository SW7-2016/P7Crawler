using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class SoundCard : ComputerComponents
    {
        string type = "";
        string speakerSupport = "";
        string socket = "";
        string fullDuplex = "";


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
                        socket = info.Value;
                        break;
                    case "full duplex":
                        fullDuplex = info.Value;
                        break;
                }
            }
        }



        public string Type
        {
            get
            {
                return type;
            }
        }

        public string SpeakerSupport
        {
            get
            {
                return speakerSupport;
            }
        }

        public string Socket
        {
            get
            {
                return socket;
            }
        }

        public string FullDuplex
        {
            get
            {
                return fullDuplex;
            }
        }
    }
}
