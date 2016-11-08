using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites
{
    abstract class Site : Host
    {
        public override abstract void Parse(string siteData);
        public override abstract void CrawlPage(string siteData);
        public override abstract string GetSiteKey(string url);
        public override abstract void CrawlReviewPages(string siteData);


        public List<string> GetReviewLinks(string siteData, string firstTagForReviewLink, string secondTagForReviewLink, string splitTag, bool reverse)
        {
            List<string> reviewLinks = new List<string>();

            string[] splitOnProducts = siteData.Split(new string[] { splitTag }, StringSplitOptions.None);
            string tempReviewLink;

            foreach (var item in splitOnProducts)
            {
                tempReviewLink = GetSearchLinks(item, firstTagForReviewLink, secondTagForReviewLink, reverse);

                if (tempReviewLink != domainUrl)
                {
                    reviewLinks.Add(tempReviewLink);
                }

            }

            return reviewLinks;
        }

    }
}
