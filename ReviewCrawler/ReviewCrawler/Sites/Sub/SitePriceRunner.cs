using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Products;
using ReviewCrawler.Products.ProductComponents;
using ReviewCrawler.Products.Retailers;
using ReviewCrawler.Helpers;

namespace ReviewCrawler.Sites.Sub
{
    class SitePriceRunner : ProductSite
    {
        public SitePriceRunner()
        {
            domainUrl = "http://www.pricerunner.dk";
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/35/Bundkort");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/40/CPU");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/37/Grafikkort");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/36/Harddiske");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/186/Kabinetter");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/48/Lydkort");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/640/Stroemforsyninger");
        }

        public override void CrawlPage(string siteData)
        {
            string tempLink = "";

            tempLink = regexMatch(siteData, "<a title=\"Næste side &raquo;\" href=\"", "\"");

            if (tempLink != "")
            {
                searchQueue.Enqueue(domainUrl + tempLink);
            }

            //Finding the review links from a page.
            string[] siteReviewLinks = regexMatches(siteData, "<a class=\"button-a\" href=\"", "\">");

            //Adding links to the queue
            foreach (string link in siteReviewLinks)
            {
                //enqueue link for price parsing
                itemQueue.Enqueue(domainUrl + link);
                //enqueue link for specification parsing 
                itemQueue.Enqueue(domainUrl + link.Replace("/pl/", "/pi/").Replace("Sammenlign-Priser", "Produkt-Info"));
            }
        }

        public override bool Parse(string siteData)
        {
            bool isDone = false;

            if (currentSite.Contains("sammenlign-priser"))
            {
                product = null;
            }

            if (product != null)
            {
            }
            else if (currentSite.Contains("/grafikkort/"))
            {
                //product is a "GPU"
                product = new GPU();
            }
            else if (currentSite.Contains("/cpu/"))
            {
                //product is a "CPU"
                product = new CPU();
            }
            else if (currentSite.Contains("/bundkort/"))
            {
                //product is a "Motherboard"
                product = new Motherboard();
            }
            else if (currentSite.Contains("/harddiske/"))
            {
                //product is a "HardDrive"
                product = new HardDrive();
            }
            else if (currentSite.Contains("/kabinetter/"))
            {
                //product is a "Chassis"
                product = new Chassis();
            }
            else if (currentSite.Contains("/stroemforsyninger/"))
            {
                //product is a "PSU"
                product = new PSU();
            }
            else
            {
                throw new FormatException("Pricerunner - couldnt determine product type", null);
            }

            if (currentSite.Contains("/pl/"))
            {
                string retailerTag = "<a rel=\"nofollow\" title=\"\" target=\"_blank\" class=\"google-analytic-retailer-data pricelink\" retailer-data=\"";

                ProductPriceRegexes pricerunnerParsePrice = new ProductPriceRegexes(
                    "<title>.*? - Sammenlign priser",
                    retailerTag + "(.*?(\n)*)*<\\/a>",
                    "<strong>kr .*?</strong>",
                    "retailer-data=\".*?\"");

                //This means that the side contains prices
                product.ParsePrice(siteData, pricerunnerParsePrice);
            }
            else if (currentSite.Contains("/pi/"))
            {
                ProductSpecRegexes pricerunnerParseSpecs = new ProductSpecRegexes(
                    "<div class=\"product-specs\">.*?</tbody>",
                    "(<tr\\s*>|<tr\\s+class=\"lastRow\">).*?</tr>",
                    "<th scope=\"row\">.*?(</th>|\\\n)",
                    "<td>.*?</td>");

                //this means that te side contains product information
                product.ParseProductSpecifications(siteData, pricerunnerParseSpecs);
                isDone = true;

            }
            else
            {
                Debug.WriteLine("Pricerunner - couldnt determine product type ");
            }

            return isDone;
        }
    }
}