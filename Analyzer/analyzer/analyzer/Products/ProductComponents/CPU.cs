using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class CPU : ComputerComponents
    {
        public string model = "";
        public string clock = "";
        public string socket = "";
        public string maxTurbo = "";
        public string integratedGpu = "";
        public string manufacturer = "";
        public string cpuSeries = "";
        public bool stockCooler;
        public int physicalCores = 0;
        public int logicalCores = 0;
    }
}