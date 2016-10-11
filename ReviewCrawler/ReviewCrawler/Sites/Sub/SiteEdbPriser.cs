using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites.Sub
{
    class SiteEdbPriser : Host
    {

        public override void CrawlPage(string siteData)
        {

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
            return url;
        }
    }
}
