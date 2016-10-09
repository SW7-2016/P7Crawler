using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;

namespace ReviewCrawler.Products
{
    abstract class Product
    {
        public string name;
        public string description;
        public decimal price;
        public byte[][] image;
        public List<Retailer> retailers;
    }
}
