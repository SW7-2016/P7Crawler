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

        public override void CrawlReviewPages(string siteData)
        {

        }

        public override void CrawlPage(string siteData)
        {

            string tempLink = "";

            string paginatorData = FindPaginator(siteData);
            int pageNumber = FindPageNumber(siteData);

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
                reviewQueue.Enqueue(link);
                reviewQueue.Enqueue(@"http://www.pricerunner.dk/pi" + GetSiteKey(link) + "-Produkt-Info");
            }
        }

        public override string GetSiteKey(string url)
        {
            for (int i = 5; i > 0; i++)
            {
                if (url[i] == 'l'
                    && url[i-1] == 'p'
                    && url[i-2] == '/')
                {
                    url = url.Remove(0, i);
                    break;
                }
            }
            for (int i = url.Length - 1; i > 0; i--)
            {
                if (url[i] == '-'
                    && url[i+1] == 's')
                {
                    url = url.Remove(i, url.Length - i);
                    break;
                }
            }
            return url;
        }

        public override void Parse(string siteData)
        {

            Product currentProduct;

            if (Crawler.products.ContainsKey(GetSiteKey(currentSite)))
            {
                currentProduct = Crawler.products[GetSiteKey(currentSite)];
            }
            else if (currentSite.Contains("/Grafikkort/"))
            {
                //product is a "GPU"
                currentProduct = new GPU(); 
            }
            else if (currentSite.Contains("/CPU/"))
            {
                //product is a "CPU"
                currentProduct = new CPU();
            }
            else if (currentSite.Contains("/Lydkort/"))
            {
                //product is a "SoundCard"
                currentProduct = new SoundCard();
            }
            else if (currentSite.Contains("/Bundkort/"))
            {
                //product is a "Motherboard"
                currentProduct = new Motherboard();
            }
            else if (currentSite.Contains("/Harddiske/"))
            {
                //product is a "HardDrive"
                currentProduct = new HardDrive();
            }
            else if (currentSite.Contains("/Kabinetter/"))
            {
                //product is a "Chassis"
                currentProduct = new Chassis();
            }
            else if (currentSite.Contains("/Stroemforsyninger/"))
            {
                //product is a "PSU"
                currentProduct = new PSU();
            }
            else
            {
                throw new FormatException("Pricerunner - couldnt determine product type", null);
            }

            if (currentSite.Contains("/pl/"))
            {
                //This means that the side contains prices
                currentProduct.ParsePrice(siteData);
            }
            else if (currentSite.Contains("/pi/"))
            {
                currentProduct.ParseProductSpecifications(siteData);
                //this means that te side contains product information

            }
            else
            {
                Debug.WriteLine("Pricerunner - couldnt determine product type ");
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
        
        //skal vi bruge denne her på hver side????
        public override string GetProductType(string tempLink)
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
                return "HardDisk"; // rename????
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
                Debug.WriteLine("Pricerunner - couldnt determine product type");
            }

            return "";
        }

        private int getPriceFromsite(string siteData)
        {
            return 5;
        }
    }
}
