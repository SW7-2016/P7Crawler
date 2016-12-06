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
        public Queue<HostInterface> hostQueue = new Queue<HostInterface>();
        public static Dictionary<string, Review> reviews = new Dictionary<string, Review>();
        public static Dictionary<string, Product> products = new Dictionary<string, Product>();

        public MySqlConnection connection =
            new MySqlConnection("server=172.25.23.57;database=crawlerdb;user=crawler;port=3306;password=Crawler23!;");

        //picks a host and starts crawling it
        public void StartCrawl(object data)
        {
            bool running = true;
            bool isHostDone = false;
            HostInterface currentHost;

            while (running)
            {
                isHostDone = false;
                if (hostQueue.Count > 0)
                {
                    currentHost = hostQueue.Dequeue();
                }
                else
                {
                    break;
                }
                

                if (PolitenessTimeCheck(currentHost.GetLastAccessTime()))
                {
                    try
                    {
                        //Starts crawling the host and returns a bool determining if the host has any more content to crawl
                        isHostDone = currentHost.StartCycle(connection);
                        currentHost.SetLastAccessTime(DateTime.Now);
                    }
                    catch (Exception E)
                    {
                        Debug.WriteLine("something went wrong with " + currentHost + ", while crawling:" + E);
                        if (hostQueue.Count > 0)
                        {
                            continue;
                        }
                        else
                        {
                            Debug.WriteLine("The crawler has stopped");
                            break;
                        }
                        
                    }
                    
                }
                else
                {
                    hostQueue.Enqueue(currentHost);
                    isHostDone = true; // to avoid enqueing twice
                    Thread.Sleep(50);
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

        //Adds all the hosts to be crawled - do this at startup
        public void AddHosts()
        {
            hostQueue.Enqueue(new SiteAmazon());
            //hostQueue.Enqueue(new SiteComputerShopper());
            //hostQueue.Enqueue(new SiteGuru3d());
            //hostQueue.Enqueue(new SitePriceRunner());
            //hostQueue.Enqueue(new SiteEdbPriser());
            //hostQueue.Enqueue(new SiteTechPowerUp());

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