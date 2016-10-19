using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;
using System.Text.RegularExpressions;

namespace ReviewCrawler.Products
{
    abstract class Product
    {
        public string name;
        public string description = "";
        protected byte[][] image;
        public List<Retailer> retailers = new List<Retailer>();

        protected abstract void AddInformation(Dictionary<string, string> productInformation);

        public void ParseProductSpecifications(string siteData)
        {
            Dictionary<string, string> productInfo = new Dictionary<string, string>();

            string rawProductInformation = (Regex.Match(siteData, "<div class=\"product-specs\">\\s*<table>.*?</table>", RegexOptions.Singleline)).Value;

            foreach (Match rawInformationRow in Regex.Matches(rawProductInformation, "(<tr\\s*>|<tr\\s+class=\"lastRow\">).*?</tr>", RegexOptions.Singleline))
            {
                // - find type of row - 
                //Used to pass sentence so that only the information is saved.(and not multible spaces and tags)
                Regex removeLongSpaces = new Regex("(\\s){2,20}");
                //Returns from "<th scope=\"row\">" to end of line
                string tempType = Regex.Match(rawInformationRow.Value, "<th scope=\"row\">.*").Value;
                tempType = tempType.Replace("<th scope=\"row\">", "");
                tempType = tempType.Replace("</th>", "");
                tempType = removeLongSpaces.Replace(tempType, "");

                // - find data of row - 
                string tempValue = Regex.Match(rawInformationRow.Value, "<td>.*?</td>").Value;
                tempValue = tempValue.Replace("<td>", "");
                tempValue = tempValue.Replace("</td>", "");

                productInfo.Add(tempType, tempValue);

            }

            AddInformation(productInfo);
        }

        public void ParsePrice(string siteData)
        {
            string retailerTag = "<a rel=\"nofollow\" title=\"\" target=\"_blank\" class=\"google-analytic-retailer-data pricelink\" retailer-data=\"";

            MatchCollection retailerCode = Regex.Matches(siteData, "(" + retailerTag + "(.*?(\n)*)*<\\/a>)+");

            foreach (Match oneRetailerCode in retailerCode)//""("
            {
                if (oneRetailerCode.Value == "") { break; }
                Retailer tempRetailer = new Retailer();

                // looking for name of retailer
                for (int i = retailerTag.Length + 1; i < 20 + retailerTag.Length; i++)
                {
                    if (oneRetailerCode.Value[i] == '(' || oneRetailerCode.Value[i] == '"')
                    {
                        tempRetailer.name = oneRetailerCode.Value.Substring(retailerTag.Length, i - retailerTag.Length);
                        break;
                    }
                }

                // looking for price of product
                string tempPrice = Regex.Match(oneRetailerCode.Value, "((<strong>).*?(<\\/strong>))+").Value;
                if (tempPrice != "") {
                    Regex regexHtml = new Regex("(<.*?>)+", RegexOptions.Singleline);
                    tempPrice = regexHtml.Replace(tempPrice, "").Replace(".", "");
                    tempPrice = tempPrice.Remove(0, 3).Replace(",", ".");
                    tempRetailer.price = decimal.Parse(tempPrice);
                }

                // looking for URL of retailer
                //Eneste link på siden, er et redirect link der går gennem pricerunner. 


                //find title of product
                name = Regex.Match(siteData, "<title>.*?-", RegexOptions.Singleline).Value.Replace("<title>", "").Replace("-", "").Trim();

                retailers.Add(tempRetailer);
            }
        }
    }
}
