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
