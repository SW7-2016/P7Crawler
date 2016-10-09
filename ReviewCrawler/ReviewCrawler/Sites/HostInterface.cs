using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites
{
    interface HostInterface
    {
        bool Crawl();
        void Parse(string siteData, string productType);
        DateTime GetLastAccessTime();
        void SetLastAccessTime(DateTime newTime);
        string GetSiteData(string siteUrl);
    }
}
