using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites
{
    interface HostInterface
    {
        bool StartCycle();
        DateTime GetLastAccessTime();
        void SetLastAccessTime(DateTime newTime);
    }
}
