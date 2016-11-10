using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class GPU : ComputerComponents
    {
        public string processorManufacturer = "";
        public string chipset = "";
        public string model = "";
        public string architecture = "";
        public string cooling = "";
        public string memSize = "";
        public string manufacturer = "";
        public int pciSlots = 0;

        GPU(int id, string category) : base(id, category)
        {
        }
    }
}
