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
            searchQueue.Enqueue("http://www.edbpriser.dk/hardware/ram.aspx?count=1000&sort=Popularity&rlm=List");
        }


        public override void CrawlPage(string siteData)
        {

            //finding next page of the review overview
            Match nextPage = Regex.Match(siteData, "<li class=\"next\"><a href=\".*?\"");
            //If there is found a next page, then add it to the queue.
            if (nextPage.Value != null)
            {
                string tempPage = nextPage.Value.Replace("<li class=\"next\"><a href=\"", "").Replace("\"", "");
                searchQueue.Enqueue(domainUrl + tempPage);
            }

            //Finding the product links from a page.
            MatchCollection newProducts = Regex.Matches(siteData, "<a class=\"link-action\" href=\".*?\"");

            //Adding all the matches of specifik products
            foreach (Match link in newProducts)
            {
                itemQueue.Enqueue(domainUrl + link.Value);
            }
        }

        public override void CrawlReviewPages(string siteData)
        {
            
        }

        public override void Parse(string siteData)
        {

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
            return url.Remove(0 , domainUrl.Length);
        }
    }
}
