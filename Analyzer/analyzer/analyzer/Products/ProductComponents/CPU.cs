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
        string model = "";
        string clock = "";
        string socket = "";
        bool stockCooler;
        string cpuSeries = "";
        int physicalCores;
        int logicalCores;
        string maxTurbo = "";
        string integratedGpu = "";
        string manufacturer = "";

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

        public string CpuSeries
        {
            get
            {
                return cpuSeries;
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