using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Sites;
using ReviewCrawler.Sites.Sub;

namespace ReviewCrawler
{
    class Crawler
    {
        public Queue<HostInterface> hostQueue = new Queue<HostInterface>();

        public void StartCrawl()
        {

        }


        public void StartCrawlThread()
        {
            Task.Factory.StartNew(() =>
            {

            });
        }
    }
}
