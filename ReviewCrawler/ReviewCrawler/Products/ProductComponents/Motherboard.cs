using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class Motherboard : ComputerComponents
    {
        string formFactor;
        string chipset;
        bool netCard;
        bool soundCard;
        bool graphicsCard;
        bool multiGpu;
        bool crossfire;
        string cpuType;
        int cpuCount;
        string socket;
        bool sli;
        int maxMem;
        int memSlots;
        string memType;

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "formfaktor":
                        formFactor = info.Value;
                        break;
                    case "socket amd":
                        socket = info.Value;
                        break;
                    case "chipset amd":
                        chipset = info.Value;
                        break;
                    case "cpu support":
                        cpuCount = int.Parse(info.Value);
                        break;
                    case "socket intel":
                        socket = info.Value;
                        break;
                    case "netværkskort indbygget":
                        netCard = (info.Value.ToLower() == "ja") ? true : false; 
                        break;
                    case "grafikkort indbygget":
                        graphicsCard = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "multiple gpu support":
                        multiGpu = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "crossfire support":
                        crossfire = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "sli support":
                        sli = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "max ram mængde":
                        maxMem = int.Parse(info.Value);
                        break;
                    case "antal dimm-pladser":
                        memSlots = int.Parse(info.Value);
                        break;
                    case "ram type":
                        memType = info.Value;
                        break;
                    case "lydkort indbygget":
                        soundCard = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "chipset others":
                        chipset = info.Value;
                        break;
                }
            }
        }
    }
}
