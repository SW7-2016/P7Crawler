using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class PSU : ComputerComponents
    {
        string power = "";
        string formFactor = "";
        bool modular;
        string brand = "";
        string height = "";
        string depth = "";
        string width = "";
        string weight = "";

        public string Power
        {
            get
            {
                return power;
            }
        }

        public string FormFactor
        {
            get
            {
                return formFactor;
            }
        }

        public bool Modular
        {
            get
            {
                return modular;
            }
        }

        public string Brand
        {
            get
            {
                return brand;
            }
        }

        public string Height
        {
            get
            {
                return height;
            }
        }

        public string Depth
        {
            get
            {
                return depth;
            }
        }

        public string Width
        {
            get
            {
                return width;
            }
        }

        public string Weight
        {
            get
            {
                return weight;
            }
        }
    }
}
