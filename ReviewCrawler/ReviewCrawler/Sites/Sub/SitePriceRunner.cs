using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites.Sub
{
    class SitePriceRunner : Host
    {

        public SitePriceRunner()
        {
            domainUrl = "http://www.pricerunner.dk";
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/35/Bundkort");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/40/CPU");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/37/Grafikkort");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/36/Harddiske");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/186/Kabinetter");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/48/Lydkort");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/38/RAM");
            searchQueue.Enqueue("http://www.pricerunner.dk/cl/640/Stroemforsyninger");
        }

        public override void CrawlPage(string currentSite, bool isReview)
        {
            string siteData = GetSiteData(currentSite);
            string tempLink = "";

            string paginatorData = FindPaginator(siteData);
            int pageNumber = FindPageNumber(currentSite);

            if (!isReview)
            {
                tempLink = GetSearchLinks(paginatorData, "<span>" + pageNumber + "</span>", "href");
                if (tempLink != domainUrl)
                {
                    searchQueue.Enqueue(tempLink);
                }
                else
                {
                    Debug.WriteLine("Line 43 SitePriveRunner: couldnt find more pages then: " + pageNumber);
                }
                
                //Finding the review links from a page.
                List<string> siteReviewLinks = GetReviewLinks(siteData);
                //Adding links to the queue
                foreach (string link in siteReviewLinks)
                {
                    reviewQueue.Enqueue(link);
                    if (currentSite.Contains("RAM"))
                    {
                        int i = 5;
                    }
                }
            }
            else if (isReview) //This is products
            {
                Parse(siteData);
            }

        }

        public List<string> GetReviewLinks(string siteData)
        {
            List<string> reviewLinks = new List<string>();
            string firstTagForReviewLink = "<p class=\"button\"><a class=\"button-a\" href=\"";
            string secondTagForReviewLink = "<a class=\"retailers\" href=\"";

            string[] splitOnProducts = siteData.Split(new string[] { "<a class=\"add\" href=\"#\" style=\"display:none\" title=\"Tilføj til min liste\">" }, StringSplitOptions.None);
            string tempReviewLink;

            foreach (var item in splitOnProducts)
            {
                tempReviewLink = GetSearchLinks(item, firstTagForReviewLink, secondTagForReviewLink);

                if (tempReviewLink != domainUrl)
                {
                    reviewLinks.Add(tempReviewLink);
                }
   
            }

            return reviewLinks;
        }

        public override void Parse(string siteData)
        {

        }

        public string FindPaginator(string data)
        {
            string result = "";
            string[] dataLines = data.ToLower().Trim().Split('\n');
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
    }
}
