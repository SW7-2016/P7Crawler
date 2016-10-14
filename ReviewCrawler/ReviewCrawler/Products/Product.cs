using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;
using System.Text.RegularExpressions;

namespace ReviewCrawler.Products
{
    abstract class Product
    {
        private string name;
        private string description;
        private decimal price;
        private byte[][] image;
        private List<Retailer> retailers = new List<Retailer>();

        public abstract void ParseProductSpecifications(string siteData);

        public void ParsePrice(string siteData)
        {
            string retailerTag = "<a rel=\"nofollow\" title=\"\" target=\"_blank\" class=\"google-analytic-retailer-data pricelink\" retailer-data=\"";

            foreach (Match item in Regex.Matches(siteData, "((pricelink\\\" retailer-data=\").*?(<\\/a>))+"))//""("
            {
                if (item.Value == "") { break; }
                Retailer tempRetailer = new Retailer();

                // looking for name of retailer
                for (int i = retailerTag.Length; i < 20 + retailerTag.Length; i++)
                {
                    if (item.Value[i] == '(' || item.Value[i] == '"')
                    {
                        tempRetailer.name = item.Value.Substring(retailerTag.Length, i / retailerTag.Length);
                        break;
                    }
                }
                // looking for price of product
                string tempPrice = Regex.Match(siteData, "((<strong class=\"validated-shipping\">).*?(<\\/strong>))+").Value;
                if (tempPrice != "") {
                    Regex regexHtml = new Regex("(<.*?>)+", RegexOptions.Singleline);
                    tempPrice = regexHtml.Replace(tempPrice, "");
                    tempPrice = tempPrice.Remove(0, 3);
                    tempRetailer.price = decimal.Parse(tempPrice);
                }

                retailers.Add(tempRetailer);
            }
        }
    }
}
