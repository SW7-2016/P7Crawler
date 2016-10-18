using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class CPU : ComputerComponents
    {
        string formFactor;
        string model;
        string clock;
        bool StockCooler;
        string cpuType;
        int physicalCores;
        int logicalCores;
        string maxTurbo;
        string integratedGpu;
        string manufacturer;

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "processor producent":
                        formFactor = info.Value;
                        break;
                }
            }
        }
    }
}