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
        private string _power = "";
        private string _formFactor = "";
        private string _brand = "";
        private string _height = "";
        private string _depth = "";
        private string _width = "";
        private string _weight = "";
        private bool _modular;

        public PSU(int id, string category, string power, string formFactor, bool modular, string width, 
                    string depth, string height, string weight, string brand) 
            : base(id, category)
        {
            _power = power;
            _formFactor = formFactor;
            _modular = modular;
            _width = width;
            _depth = depth;
            _height = height;
            _weight = weight;
            _brand = brand;
        }

    }
}
