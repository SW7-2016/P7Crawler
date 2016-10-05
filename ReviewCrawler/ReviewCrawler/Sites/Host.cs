using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ReviewCrawler.Sites
{
    abstract class Host : HostInterface
    {
        public DateTime timeStamp = DateTime.Now;
        public Queue<string> reviewQueue = new Queue<string>();
        public Queue<string> searchQueue = new Queue<string>();


        public abstract void Parse();
        public abstract void CrawlPage(string currentSite);

        public bool Crawl()
        {

            string currentSite = "";

            if (searchQueue.Count > 0)
            {
                currentSite = searchQueue.Dequeue();
            }
            else if (reviewQueue.Count > 0)
            {
                currentSite = reviewQueue.Dequeue();
            }

            CrawlPage(currentSite); //Handles the site specific crawling, is overwritten in subclasses

            //Checks if there is more content to crawl on this host
            if (reviewQueue.Count < 1 && searchQueue.Count < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DateTime GetLastAccessTime()
        {
            return timeStamp;
        }

        public string GetSiteData(string siteUrl)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Proxy = null;
            byte[] raw;
            string webData = "";

            try
            {
                raw = wc.DownloadData(siteUrl);

                webData = Encoding.UTF8.GetString(raw);
            }
            catch (Exception E)
            {
                Debug.WriteLine("failed to get data from: " + siteUrl);
            }

            return webData;
        }
    }
}

