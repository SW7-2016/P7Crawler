using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class CPU : ComputerComponents
    {
        string model = "";
        string clock = "";
        string socket = "";
        bool stockCooler;
        string cpuSerie = "";
        int physicalCores;
        int logicalCores;
        string maxTurbo = "";
        string integratedGpu = "";
        string manufacturer = "";

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "processor serie":
                        cpuSerie = info.Value;
                        break;
                    case "processor model":
                        model = info.Value;
                        break;
                    case "clockfrekvens":
                        clock = info.Value;
                        break;
                    case "integreret gpu":
                        integratedGpu = info.Value;
                        break;
                    case "boxed (inkluderer blæser eller køler)":
                        stockCooler = (info.Value.ToLower() == "ja") ? true : false; ;
                        break;
                    case "mærke":
                        manufacturer = Regex.Replace(info.Value, "(<.*?>)+", "");
                        break;
                    case "processorkerner":
                        Match noOfCores = Regex.Match(info.Value, "\\d*");
                        physicalCores = int.Parse(noOfCores.Value);
                        break;
                    case "processor tråde":
                        Match noOfThreads = Regex.Match(info.Value, "\\d*");
                        logicalCores = int.Parse(noOfThreads.Value);
                        break;
                    case "max turbo frequency":
                        maxTurbo = info.Value;
                        break;
                    case "sokkel":
                        socket = info.Value;
                        break;
                }
            }
        }
        public string Model
        {
            get
            {
                return model;
            }
        }

        public string Clock
        {
            get
            {
                return clock;
            }
        }

        public string Socket
        {
            get
            {
                return socket;
            }
        }

        public bool StockCooler
        {
            get
            {
                return stockCooler;
            }
        }

        public string CpuSerie
        {
            get
            {
                return cpuSerie;
            }
        }

        public int PhysicalCores
        {
            get
            {
                return physicalCores;
            }
        }

        public int LogicalCores
        {
            get
            {
                return logicalCores;
            }
        }

        public string MaxTurbo
        {
            get
            {
                return maxTurbo;
            }
        }

        public string IntegratedGpu
        {
            get
            {
                return integratedGpu;
            }
        }

        public string Manufacturer
        {
            get
            {
                return manufacturer;
            }
        }
    }
}