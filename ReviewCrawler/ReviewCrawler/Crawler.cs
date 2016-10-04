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

        //picks a host and starts crawling it
        public void StartCrawl()
        {
            bool running = true;
            HostInterface currentHost;

            while (running)
            {
                currentHost = hostQueue.Dequeue();

                if (PolitenessTimeCheck(currentHost.GetLastAccessTime()))
                {
                    currentHost.Crawl();
                }
                else
                {
                    hostQueue.Enqueue(currentHost);
                }
            }
            
        }

        

        public void StartCrawlThread()
        {
            
        }

        public void AddHosts()
        {
            hostQueue.Enqueue(new SiteGuru3d());

        }

        //Checks if more than two seconds have passed since 'lastAccessTime' and returns a bool
        public bool PolitenessTimeCheck(DateTime lastAccessTime)
        {
            if ((DateTime.Now - lastAccessTime).TotalSeconds > 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
