using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites.Sub
{
    class SitePcWorld : ReviewSite
    {

        public override void CrawlPage(string siteData, string sQueueData)
        {

        }


        public override bool Parse(string siteData, string sQueueData)
        {
            return false;
        }

        public override string GetProductType(string tempLink)
        {
            return "";
        }

    }
}
