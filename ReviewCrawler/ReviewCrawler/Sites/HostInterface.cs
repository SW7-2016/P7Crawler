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
        void Parse(string siteData);
        DateTime GetLastAccessTime();
        string GetSiteData(string siteUrl);

    }
}
