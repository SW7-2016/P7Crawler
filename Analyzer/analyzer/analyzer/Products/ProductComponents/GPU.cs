using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class GPU : ComputerComponents
    {
        string processorManufacturer = "";
        string chipset = "";
        string model = "";
        string architecture = "";
        int pciSlots;
        string cooling = "";
        string memSize = "";
        string manufacturer = "";


        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "processor producent":
                        processorManufacturer = info.Value;
                        break;
                    case "geforce serie":
                        model = info.Value;
                        break;
                    case "chipset":
                        chipset = info.Value;
                        break;
                    case "gpu architecture":
                        architecture = info.Value;
                        break;
                    case "kortpladser":
                        pciSlots = int.Parse(info.Value);
                        break;
                    case "køling":
                        cooling = info.Value;
                        break;
                    case "hukommelsesstørrelse":
                        memSize = info.Value;
                        break;
                    case "radeon serie":
                        model = info.Value;
                        break;
                }
            }
        }
        public string ProcessorManufacturer
        {
            get
            {
                return processorManufacturer;
            }
        }

        public string Chipset
        {
            get
            {
                return chipset;
            }
        }

        public string Model
        {
            get
            {
                return model;
            }
        }

        public string Architecture
        {
            get
            {
                return architecture;
            }
        }

        public int PciSlots
        {
            get
            {
                return pciSlots;
            }
        }

        public string Cooling
        {
            get
            {
                return cooling;
            }
        }

        public string MemSize
        {
            get
            {
                return memSize;
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
