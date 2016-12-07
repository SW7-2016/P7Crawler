using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Sites
{
    interface HostInterface
    {
        bool CrawlParseCycle(MySqlConnection connection);
        DateTime GetLastAccessTime();
        void SetLastAccessTime(DateTime newTime);
    }
}
