using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class HardDrive : ComputerComponents
    {
        private bool _isInternal;
        private string _type = "";
        private string _formFactor = "";
        private string _capacity = "";
        private string _cacheSize = "";
        private string _transferRate = "";
        private string _brand = "";
        private string _sata = "";
        private string _height = "";
        private string _depth = "";
        private string _width = "";

        public HardDrive(int id, string category, bool isInternal, string type, string formFactor, string capacity, string cacheSize, 
                        string transferRate, string brand, string sata, string height, string depth, string width) 
            : base(id, category)
        {
            _isInternal = isInternal;
            _type = type;
            _formFactor = formFactor;
            _capacity = capacity;
            _cacheSize = cacheSize;
            _transferRate = transferRate;
            _brand = brand;
            _sata = sata;
            _height = height;
            _depth = depth;
            _width = width;
        }
    }
}
