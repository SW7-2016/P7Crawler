using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Helpers
{
    class ProductPriceRegexes
    {
        public string productTitlePattern;
        public string allRetailersPattern;
        public string retailerPricePattern;
        public string retailerNamePattern;

        public ProductPriceRegexes(string ProductTitlePattern, string AllRetailersPattern, string RetailerPricePattern, string RetailerNamePattern)
        {
            productTitlePattern = ProductTitlePattern;
            allRetailersPattern = AllRetailersPattern;
            retailerPricePattern = RetailerPricePattern;
            retailerNamePattern = RetailerNamePattern;
        }
    }
}
