using System.Collections.Generic;
using analyzer.Products.Reviews;
using analyzer.Products.Retailers;
using System.Text.RegularExpressions;

namespace analyzer.Products
{
    abstract class Product
    {
        public int id = 0;
        public double superScore = 0;
        public double criticScore = 0;
        public double userScore = 0;
        public string category = "";
        public string name = "";
        public string description = "";
        protected byte[][] image;
        public List<Retailer> retailers = new List<Retailer>();
    }
}
