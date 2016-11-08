using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites
{
    abstract class ReviewSite : Site
    {
        public override abstract void Parse(string siteData);
        public override abstract void CrawlPage(string siteData);
        public override abstract string GetSiteKey(string url);
        public override abstract void CrawlReviewPages(string siteData);
    }
}
