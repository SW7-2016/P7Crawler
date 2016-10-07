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
            bool isHostDone = false;
            HostInterface currentHost;

            while (running)
            {
                isHostDone = false;
                currentHost = hostQueue.Dequeue();

                if (PolitenessTimeCheck(currentHost.GetLastAccessTime()))
                {
                     //Starts crawling the host and returns a bool determining if the host has any more content to crawl
                     isHostDone = currentHost.Crawl();
                }
                else
                {
                    hostQueue.Enqueue(currentHost);
                }

                //Requeues the host if it has more pages to be crawled
                if (!isHostDone)
                {
                    hostQueue.Enqueue(currentHost);
                }

                //Stops if the hostQueue is empty
                if (hostQueue.Count < 1)
                {
                    running = false;
                }
            }
            
        }

        //Adds all the hosts to be crawled - do this at startup
        public void AddHosts()
        {
            hostQueue.Enqueue(new SiteGuru3d());
            //hostQueue.Enqueue(new SitePriceRunner());
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
