using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Helpers;

namespace ReviewCrawler.Sites.Sub
{
    class SiteAmazon : ReviewSite
    {
        public SiteAmazon()
        {
            domainUrl = "https://www.amazon.com";
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_284822_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A284822&ie=UTF8&qid=1479199524&lo=computers", ""));
        }

        public override void CrawlPage(string siteData, string sQueueData)
        {
            //(href=".*?"><h2 data-attribute)
            string nextPageLink = "";
            string[] articleLinks;
            string tempProductType = "unknown";

            //if (currentSite.Contains("articles-categories"))
            //{
            //Gets match, without identifiers.
            nextPageLink = regexMatch(siteData, "class=\"pagnNext\"", "<span id=\"pagnNextString\">Next Page</span>");
            nextPageLink = regexMatch(nextPageLink, "href=\"", "\">");
            
                if (nextPageLink != "")
                {
                    searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink, null));
                }
           // }

            //Gets matches, without identifiers.
            articleLinks = regexMatches(siteData, "href=\"", "\"><h2 data-attribute");

            foreach (string link in articleLinks)
            {
                searchQueue.Enqueue(new QueueElement(domainUrl + link, ""));
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