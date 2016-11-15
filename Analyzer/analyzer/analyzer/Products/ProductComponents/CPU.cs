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
        private string _model = "";
        private string _clock = "";
        private string _socket = "";
        private string _maxTurbo = "";
        private string _integratedGpu = "";
        private string _manufacturer = "";
        private string _cpuSeries = "";
        private bool _stockCooler;
        private int _physicalCores = 0;
        private int _logicalCores = 0;

        public CPU(string category, int id, string model, string clock, string maxTurbo, string integratedGpu,
                    bool stockCooler, string manufacturer, string cpuSeries, int logicalCores, int physicalCores)
            : base(id, category)
        {
            _model = model;
            _clock = clock;
            _maxTurbo = maxTurbo;
            _integratedGpu = integratedGpu;
            _stockCooler = stockCooler;
            _manufacturer = manufacturer;
            _cpuSeries = cpuSeries;
            _logicalCores = logicalCores;
            _physicalCores = physicalCores;
        }
    }
}