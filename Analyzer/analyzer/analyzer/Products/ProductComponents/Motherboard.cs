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
        private string _formFactor = "";
        private string _chipset = "";
        private string _cpuType = "";
        private string _socket = "";
        private string _memType = "";
        private bool _netCard;
        private bool _soundCard;
        private bool _supportIntegratedGraphicsCard;
        private bool _multiGpu;
        private bool _crossfire;
        private bool _sli;
        private int _cpuCount = 0;
        private int _maxMem = 0;
        private int _memSlots = 0;

        public Motherboard(string category, int id, string formFactor, string cpuType, int cpuCount, string socket, 
                            bool netCard, bool soundCard, bool multiGPU, bool crossfire, bool sli, int maxMem, 
                            int memSlots, string memType, bool supportIntegratedGraphicsCard, string chipset) 
            : base(id, category)
        {
            _formFactor = formFactor;
            _cpuType = cpuType;
            _cpuCount = cpuCount;
            _socket = socket;
            _netCard = netCard;
            _soundCard = soundCard;
            _multiGpu = multiGPU;
            _crossfire = crossfire;
            _sli = sli;
            _maxMem = maxMem;
            _memSlots = memSlots;
            _memType = memType;
            _supportIntegratedGraphicsCard = supportIntegratedGraphicsCard;
            _chipset = chipset;
        }
    }
}
