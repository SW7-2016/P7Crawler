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
            bool running = true;
            HostInterface currentHost;

            while (running)
            {
                currentHost = hostQueue.Dequeue();
            }
            
        }


        public void StartCrawlThread()
        {
            
        }

        public void AddHosts()
        {
            hostQueue.Enqueue(new SiteGuru3d());

        }

    }
}
