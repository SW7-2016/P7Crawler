using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;
using System.Text.RegularExpressions;
using System;

namespace ReviewCrawler.Products
{

    //HUSK at ram indeholder 2x model. fix det.
    abstract class Product
    {
        public string name;
        public string description = "";
        protected byte[][] image;
        public List<Retailer> retailers = new List<Retailer>();

        protected abstract void AddInformation(Dictionary<string, string> productInformation);

        public void ParseProductSpecifications(string siteData, Dictionary<string, string> regexPatterns)
        {
            Dictionary<string, string> productInfo = new Dictionary<string, string>();

            MatchCollection rawProductInformation = (Regex.Matches(siteData, regexPatterns["table"], RegexOptions.Singleline));

            foreach (Match informationTable in rawProductInformation)
            {
                foreach (Match rawInformationRow in Regex.Matches(informationTable.Value, regexPatterns["spec"], RegexOptions.Singleline))
                {
                    Regex removeTags = new Regex("(<.*?>)+", RegexOptions.Singleline);

                    // - find type of row - 
                    string tempType = removeTags.Replace(Regex.Match(rawInformationRow.Value, regexPatterns["spec name"]).Value, "").Trim();

                    // - find data of row - 
                    string tempValue = removeTags.Replace(Regex.Match(rawInformationRow.Value, regexPatterns["spec value"]).Value, "").Trim();

                    productInfo.Add(tempType, tempValue);
                }
            }

            AddInformation(productInfo);
            //databasethis(this);
        }

        public void ParsePrice(string siteData, Dictionary<string, string> regexPatterns)
        {
            //find title of product
            name = Regex.Match(siteData, regexPatterns["title"], RegexOptions.Singleline).Value.Replace("<title>", "").Replace("- Sammenlign priser", "").Trim();

            // Find retailers and add to product
            foreach (Match oneRetailerCode in Regex.Matches(siteData, regexPatterns["all retailers"]))
            {
                Retailer tempRetailer = new Retailer();

                // Finding retailer name.
                Regex split = new Regex("\\.\\*\\?");
                string[] tags = split.Split(regexPatterns["retailer name"]);
                tempRetailer.name = Regex.Match(oneRetailerCode.Value, regexPatterns["retailer name"]).Value.Replace(tags[0], "").Replace(tags[1], "");

                if (tempRetailer.name != "")
                {
                    // Finding retailer price
                    Regex removeTags = new Regex("(<.*?>)+", RegexOptions.Singleline);
                    string tempPrice = Regex.Match(oneRetailerCode.Value, regexPatterns["retailer price"]).Value;

                    tempPrice = removeTags.Replace(tempPrice, "").Replace(".", "");
                    tempPrice = tempPrice.Replace("kr", "").Trim().Replace(",", ".");
                    tempRetailer.price = decimal.Parse(tempPrice);

                    retailers.Add(tempRetailer);
                }
            }
        }
    }
}
