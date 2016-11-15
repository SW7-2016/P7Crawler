using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class RAM : ComputerComponents
    {
        private string _type = "";
        private string _capacity = "";
        private string _speed = "";
        private string _technology = "";
        private string _formFactor = "";
        private string _casLatency = "";

        public RAM(int id, string category, string type, string capacity, string speed, 
                    string technology, string formFactor, string casLatency) 
            : base(id, category)
        {
            _type = type;
            _capacity = capacity;
            _speed = speed;
            _technology = technology;
            _formFactor = formFactor;
            _casLatency = casLatency;
        }
    }
}
