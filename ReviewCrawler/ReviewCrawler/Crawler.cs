using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Sites;
using ReviewCrawler.Sites.Sub;
using ReviewCrawler.Products;
using ReviewCrawler.Products.Reviews;
using System.Threading;
using MySql.Data.MySqlClient;
using ReviewCrawler.Helpers;

namespace ReviewCrawler
{
    class Crawler
    {
        private Queue<HostInterface> hostQueue = new Queue<HostInterface>(); //Contains all the different hosts
        private readonly MySqlConnection connection =
            new MySqlConnection("server=172.25.23.57;database=crawlerdb;user=crawler;port=3306;password=Crawler23!;");

        //Picks a host and starts crawling it
        public void StartCrawlCycle(object data)
        {
            bool running = true;
            HostInterface currentHost;

            while (running)
            {
                bool isHostDone = false;
                if (hostQueue.Count > 0) //Makes sure that the crawler only continues if the host queue isn't empty
                {
                    currentHost = hostQueue.Dequeue(); //Dequeues the next host
                }
                else
                {
                    Debug.WriteLine("The crawler has stopped");
                    break;
                }
                //Checks if enough time has passed since last visit to this host
                if (PolitenessTimeCheck(currentHost.GetLastAccessTime()))
                {
                    try
                    {
                        //Starts crawling the host and returns a bool determining if the host has any more content to crawl
                        isHostDone = currentHost.CrawlParseCycle(connection);
                        currentHost.SetLastAccessTime(DateTime.Now); //updates last access time
                    }
                    catch (Exception E)
                    {
                        Debug.WriteLine("something went wrong with " + currentHost + ", while crawling:" + E);
                        continue;
                    }
                }
                else //if politeness is not satisfied
                {
                    hostQueue.Enqueue(currentHost);
                    isHostDone = true; //to avoid enqueing the host again in the if statement below
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
                    Debug.WriteLine("The crawler is finished");
                }
            }
        }

        //Adds all the hosts to be crawled
        public void AddHosts()
        {
            hostQueue.Enqueue(new SiteAmazon());
            hostQueue.Enqueue(new SiteComputerShopper());
            hostQueue.Enqueue(new SiteGuru3d());
            hostQueue.Enqueue(new SitePriceRunner());
            hostQueue.Enqueue(new SiteEdbPriser());
            hostQueue.Enqueue(new SiteTechPowerUp());
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