using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class Chassis : ComputerComponents
    {
        string type = "";
        bool atx;
        bool miniAtx;
        bool miniItx;
        string fans = "";
        string brand = "";
        string weight = "";
        string height = "";
        string depth = "";
        string width = "";

        public string Type
        {
            get
            {
                return type;
            }
        }

        public string Fans
        {
            get
            {
                return fans;
            }
        }

        public bool Atx
        {
            get
            {
                return atx;
            }
        }

        public bool MiniAtx
        {
            get
            {
                return miniAtx;
            }
        }

        public string Brand
        {
            get
            {
                return brand;
            }
        }

        public string Weight
        {
            get
            {
                return weight;
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

        public bool MiniItx
        {
            get
            {
                return miniItx;
            }
        }

    }
}
