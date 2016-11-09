using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    class Cooling : ComputerComponents
    {
        string type = "";
        string speed = "";
        string size = "";
        string airflow = "";
        string noise = "";
        string connector = "";

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

        public string Speed
        {
            get
            {
                return speed;
            }
        }

        public string Size
        {
            get
            {
                return size;
            }
        }

        public string Airflow
        {
            get
            {
                return airflow;
            }
        }

        public string Noise
        {
            get
            {
                return noise;
            }
        }

        public string Connector
        {
            get
            {
                return connector;
            }
        }
    }
}
