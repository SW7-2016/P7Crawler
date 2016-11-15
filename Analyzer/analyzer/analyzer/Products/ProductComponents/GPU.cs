using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class GPU : ComputerComponents
    {
        private string _processorManufacturer = "";
        private string _chipset = "";
        private string _model = "";
        private string _architecture = "";
        private string _cooling = "";
        private string _memSize = "";
        private string _manufacturer = "";
        private int _pciSlots = 0;

        public GPU(string category, int id, string processorManufacturer, string chipset, string model, string architecture,
                                    string cooling, string memSize, int pciSlots, string manufacturer) 
            : base(id, category)
        {
            _processorManufacturer = processorManufacturer;
            _chipset = chipset;
            _model = model;
            _architecture = architecture;
            _cooling = cooling;
            _memSize = memSize;
            _manufacturer = manufacturer;
            _pciSlots = pciSlots;
        }
    }
}
