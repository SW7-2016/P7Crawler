using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class Motherboard : ComputerComponents
    {
        public string formFactor = "";
        public string chipset = "";
        public string cpuType = "";
        public string socket = "";
        public string memType = "";
        public bool netCard;
        public bool soundCard;
        public bool supportIntegratedGraphicsCard;
        public bool multiGpu;
        public bool crossfire;
        public bool sli;
        public int cpuCount = 0;
        public int maxMem = 0;
        public int memSlots = 0;
    }
}
