using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;

namespace ReviewCrawler.Products
{
    abstract class Product
    {
        private string name;
        private string description;
        private decimal price;
        private byte[][] image;
        private List<Retailer> retailers;

        public abstract void ParseProductSpecifications(string siteData);

        public void ParsePrice(string siteData)
        {

        }
    }
}
