using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Products;
using ReviewCrawler.Products.ProductComponents;
using ReviewCrawler.Products.Retailers;

namespace ReviewCrawler.Sites.Sub
{
    class SitePriceRunner : Host
    {
        string currentSite;

        public SitePriceRunner()
        {
            domainUrl = "http://www.pricerunner.dk";
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/35/Bundkort");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/40/CPU");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/37/Grafikkort");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/36/Harddiske");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/186/Kabinetter");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/48/Lydkort");
            //searchQueue.Enqueue("http://www.pricerunner.dk/cl/640/Stroemforsyninger");
        }

        public override void CrawlPage(string CurrentSite)
        {
            currentSite = CurrentSite;

            string siteData = GetSiteData(currentSite);
            string tempLink = "";

            string paginatorData = FindPaginator(siteData);
            int pageNumber = FindPageNumber(currentSite);

            tempLink = GetSearchLinks(paginatorData, "<span>" + pageNumber + "</span>", "href", false);
            if (tempLink != domainUrl)
            {
                searchQueue.Enqueue(tempLink);
            }
            else
            {
                Debug.WriteLine("Line 43 SitePriceRunner: couldnt find more pages then: " + pageNumber);
            }

            //Finding the review links from a page.
            List<string> siteReviewLinks = GetReviewLinks(siteData
                , "<p class=\"button\"><a class=\"button-a\" href=\""
                , "<a class=\"retailers\" href=\""
                , "<a class=\"add\" href=\"#\" style=\"display:none\" title=\"Tilføj til min liste\">"
                , false);
            //Adding links to the queue
            foreach (string link in siteReviewLinks)
            {
                reviewQueue.Enqueue(new KeyValuePair<string, string>(link, GetProductType(link)));
            }
        }

        private string getSpecLink(string siteData)
        {
            return "link";
        }

        private Product ParseCPU(string[] siteData)
        {
            return new CPU();
        }

        private List<Retailer> getRetailersFromSite(string siteData)
        {
            return new List<Retailer>();
        }

        public override void Parse(string siteData, string productType)
        {
            string[] dataLines = siteData.ToLower().Trim().Split('\n');

            Product newProduct;

            if (productType == "CPU")
            {
                string specSite = getSpecLink(siteData);

                newProduct = ParseCPU(GetSiteData(specSite).Split('\n'));

                newProduct.retailers.AddRange(getRetailersFromSite(siteData));
            }
            else if (productType == "GPU")
            {

            }
            else if (productType == "SoundCard")
            {

            }
            else if (productType == "Motherboard")
            {

            }
            else if (productType == "HardDisk")
            {

            }
            else if (productType == "Chassis")
            {

            }
            else if (productType == "PSU")
            {

            }
        }

        public string FindPaginator(string siteData)
        {
            string result = "";
            string[] dataLines = siteData.ToLower().Trim().Split('\n');
            bool isNeeded = false;

            foreach (string line in dataLines)
            {
                if (line.Contains("<div class=\"paginator\">"))
                {
                    isNeeded = true;
                }
                if (isNeeded)
                {
                    result += line + '\n';
                }
                if (isNeeded && line.Contains("</div>"))
                {
                    break;
                }
            }

            return result;
        }

        public int FindPageNumber(string url)
        {
            if (url.Contains("?page="))
            {
                string tempPageNumber = "";
                bool eqFound = false;

                for (int i = 0; i < url.Count(); i++)
                {
                    if (eqFound)
                    {
                        tempPageNumber += url[i];
                    }
                    if (url[i] == '=')
                    {
                        eqFound = true;
                    }
                }

                return Convert.ToInt32(tempPageNumber);
            }
            else
            {
                return 1;
            }
        }

        private string GetProductType(string tempLink)
        {

            if (tempLink.Contains("/Grafikkort/"))
            {
                return "GPU";
            }
            else if (tempLink.Contains("/CPU/"))
            {
                return "CPU";
            }
            else if (tempLink.Contains("/Lydkort/"))
            {
                return "SoundCard";
            }
            else if (tempLink.Contains("/Bundkort/"))
            {
                return "Motherboard";
            }
            else if (tempLink.Contains("/Harddiske/"))
            {
                return "HardDisk";
            }
            else if (tempLink.Contains("/Kabinetter/"))
            {
                return "Chassis";
            }
            else if (tempLink.Contains("/Stroemforsyninger/"))
            {
                return "PSU";
            }
            else
            {
                Debug.WriteLine("couldnt determine product type - GetProductType, guru3d");
            }

            return "";
        }

        private int getPriceFromsite(string siteData)
        {
            return 5;
        }
    }
}
