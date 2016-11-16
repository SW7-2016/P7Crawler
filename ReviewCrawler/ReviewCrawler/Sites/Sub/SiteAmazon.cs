using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Helpers;
using ReviewCrawler.Products.Reviews;

namespace ReviewCrawler.Sites.Sub
{
    class SiteAmazon : ReviewSite
    {
        List<Review> reviewList = new List<Review>();
        public SiteAmazon()
        {
            domainUrl = "https://www.amazon.com";
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_284822_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A284822&ie=UTF8&qid=1479199524&lo=computers", "state1"));
        }

        public override void CrawlPage(string siteData, string sQueueData)
        {
            string nextPageLink = "";
            string[] articleLinks;
            string tempProductType = "unknown";

            if (sQueueData.Contains("state1"))
            {
            //Gets match, without identifiers.
            nextPageLink = regexMatch(siteData, "class=\"pagnNext\"", "<span id=\"pagnNextString\">Next Page</span>");
            nextPageLink = regexMatch(nextPageLink, "href=\"", "\">");
            
                if (nextPageLink != "")
                {
                    searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink, "state1"));
                }
                //Gets matches, without identifiers.
                articleLinks = regexMatches(siteData, "href=\"", "\"><h2 data-attribute");

                foreach (string link in articleLinks)
                {
                    searchQueue.Enqueue(new QueueElement(domainUrl + link, "state2"));
                }
            }
            else if (sQueueData.Contains("state2"))
            {
                
            }

            
            // CrawlReviewPages(siteData);

            //<a class="a-link-emphasis a-text-bold" href="
            //">
        }

        public override bool Parse(string siteData, string sQueueData)
        {
            return false;
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