using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products
{
    class Motherboard : ComputerComponents
    {
        string formFactor;
        string chipset;
        bool netCard;
        bool soundCard;
        bool graphicsCard;
        bool multiGpu;
        bool crossfire;
        string cpuType;
        int cpuCount;
        string socket;
        string sli;
        string maxMem;
        string memSlots;
        string memType;
    }
}
