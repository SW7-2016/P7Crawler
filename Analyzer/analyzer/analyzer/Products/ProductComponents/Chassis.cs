using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class Chassis : ComputerComponents
    {
        private string _type = "";
        private string _brand = "";
        private string _weight = "";
        private string _height = "";
        private string _depth = "";
        private string _width = "";
        private string _fans = "";
        private bool _atx;
        private bool _miniAtx;
        private bool _miniItx;

        public Chassis(string category, int id, string type, bool atx, bool miniAtx, bool miniItx,
                    string fans, string brand, string height, string width, string depth, string weight)
            : base(id, category)
        {
            _type = type;
            _atx = atx;
            _miniAtx = miniAtx;
            _miniItx = miniItx;
            _fans = fans;
            _brand = brand;
            _height = height;
            _width = width;
            _depth = depth;
            _weight = weight;
        }
    }
}
