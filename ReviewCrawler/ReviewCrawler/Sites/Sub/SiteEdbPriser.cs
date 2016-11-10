using ReviewCrawler.Products;
using ReviewCrawler.Products.ProductComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites.Sub
{
    class SiteEdbPriser : ProductSite
    {
        public SiteEdbPriser()
        {
            domainUrl = "http://www.edbpriser.dk";
            searchQueue.Enqueue("http://www.edbpriser.dk/hardware/ram.aspx?count=500&sort=Popularity&rlm=List");
        }


        public override void CrawlPage(string siteData)
        {

            //finding next page of the review overview
            Match nextPage = Regex.Match(siteData, "<li class=\"next\"><a href=\".*?\"");
            //If there is found a next page, then add it to the queue.
            if (nextPage.Value != "")
            {
                string tempPage = nextPage.Value.Replace("<li class=\"next\"><a href=\"", "").Replace("\"", "");
                //searchQueue.Enqueue(domainUrl + tempPage);
            }

            //Finding the product links from a page.
            MatchCollection newProducts = Regex.Matches(siteData, "<a class=\"link-action\" href=\".*?\"");

            //Adding all the matches of specifik products
            foreach (Match link in newProducts)
            {
                itemQueue.Enqueue(domainUrl + link.Value.Replace("<a class=\"link-action\" href=\"", "").Replace("\"", ""));
            }
        }

        public override bool Parse(string siteData)
        {
            Dictionary<string, string> regexPatternsPPS = new Dictionary<string, string>()
            {
                { "table", "<td class=\"headline\" colspan=.*?</table>"},
                { "spec", "<tr class=\"sec\">.*?</tr>"},
                { "spec name", "<td class=\"spec\">.*?</td>"},
                { "spec value", "<td>.*?</td>"},
            };

            Dictionary<string, string> regexPatternsPP = new Dictionary<string, string>()
            {
                { "title", "<h1 class=\"product-details-header\" itemprop=\"name\">.*?</h1>"},
                { "all retailers", "<div class=\"ProductDealerList\">.*?</tbody>"},
                { "retailer price", "<td><strong>.*? kr</strong></td>"},
                { "reatiler name", "<div class=\"vendor-name\">.*?</div>"},
            };

            Product currentProduct;

            if (Crawler.products.ContainsKey(GetSiteKey(currentSite)))
            {
                currentProduct = Crawler.products[GetSiteKey(currentSite)];
            }
            else if (currentSite.Contains("/grafikkort"))
            {
                //product is a "GPU"
                currentProduct = new GPU();
            }
            else if (currentSite.Contains("/cpu"))
            {
                //product is a "CPU"
                currentProduct = new CPU();
            }
            else if (currentSite.Contains("/bundkort"))
            {
                //product is a "Motherboard"
                currentProduct = new Motherboard();
            }
            else if (currentSite.Contains("/harddiske"))
            {
                //product is a "HardDrive"
                currentProduct = new HardDrive();
            }
            else if (currentSite.Contains("/kabinetter"))
            {
                //product is a "Chassis"
                currentProduct = new Chassis();
            }
            else if (currentSite.Contains("/stroemforsyninger"))
            {
                //product is a "PSU"
                currentProduct = new PSU();
            }
            else if (currentSite.Contains("/ram"))
            {
                //product is a "PSU"
                currentProduct = new PSU();
            }
            else
            {
                throw new FormatException("Pricerunner - couldnt determine product type", null);
            }

            currentProduct.ParsePrice(siteData, regexPatternsPP);
            currentProduct.ParseProductSpecifications(siteData, regexPatternsPPS);

            //edbpriser match title -> <h1 class=\"product-details-header\" itemprop=\"name\">.*?</h1>

            //edbpriser match alle retailers -> <div class=\"ProductDealerList\">.*?
            //edbpriser matches pris -> <td><strong>.*? kr</strong></td>
            //edbpriser matches navn -> <div class=\"vendor-name\">.*?</div>   + html remove (<REMOVE>) + trim() + TEST DET

            //edbpriser matches spec tables -> <td class=\"headline\" colspan=.*?</table>
            //edbpriser matches specs ->  <tr class=\"sec\">.*?</tr>
            //edbpriser match spec navn -> <td class=\"spec\">.*?</td>
            //edbpriser match spec value -> <td>.*?</td>

            return true;
        }

        public override string GetProductType(string tempLink)
        {
            return "";
        }

        public override string GetSiteKey(string url)
        {
            /*for (int i = url.Length; i > 0; i--)
            {
                if (url[i] == ',')
                {
                    url = url.Remove(i, url.Length - i);

                }
            }
            */
            return url;
        }
    }
}

