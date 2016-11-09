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
        string formFactor = "";
        string chipset = "";
        bool netCard;
        bool soundCard;
        bool graphicsCard;
        bool multiGpu;
        bool crossfire;
        string cpuType = "";
        int cpuCount;
        string socket = "";
        bool sli;
        int maxMem;
        int memSlots;
        string memType = "";

        public string FormFactor
        {
            get
            {
                return formFactor;
            }
        }

        public string Chipset
        {
            get
            {
                return chipset;
            }
        }

        public bool NetCard
        {
            get
            {
                return netCard;
            }
        }

        public bool SoundCard
        {
            get
            {
                return soundCard;
            }
        }

        public bool GraphicsCard
        {
            get
            {
                return graphicsCard;
            }
        }

        public bool MultiGpu
        {
            get
            {
                return multiGpu;
            }
        }

        public bool Crossfire
        {
            get
            {
                return crossfire;
            }
        }

        public string CpuType
        {
            get
            {
                return cpuType;
            }
        }

        public int CpuCount
        {
            get
            {
                return cpuCount;
            }
        }

        public string Socket
        {
            get
            {
                return socket;
            }
        }

        public bool Sli
        {
            get
            {
                return sli;
            }
        }

        public int MaxMem
        {
            get
            {
                return maxMem;
            }
        }

        public int MemSlots
        {
            get
            {
                return memSlots;
            }
        }

        public string MemType
        {
            get
            {
                return memType;
            }
        }
    }
}
