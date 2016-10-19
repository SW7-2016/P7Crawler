using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class RAM : ComputerComponents
    {
        string type = "";
        string capacity = "";
        string speed = "";
        string technology = "";
        string formFactor = "";
        string casLatens = "";


        protected override void AddInformation(Dictionary<string, string> productInformation)
        {

        }


        public string Type
        {
            get
            {
                return type;
            }
        }

        public string Capacity
        {
            get
            {
                return capacity;
            }
        }

        public string Speed
        {
            get
            {
                return speed;
            }
        }

        public string Technology
        {
            get
            {
                return technology;
            }
        }

        public string FormFactor
        {
            get
            {
                return formFactor;
            }
        }

        public string CasLatens
        {
            get
            {
                return casLatens;
            }
        }
    }
}
