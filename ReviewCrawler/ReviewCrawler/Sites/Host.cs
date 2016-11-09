using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ReviewCrawler.Products;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Sites.Sub;
using System.Text.RegularExpressions;

namespace ReviewCrawler.Sites
{
    abstract class Host : HostInterface
    {
        protected DateTime visitTimeStamp = DateTime.Now;
        protected DateTime robotsTimeStamp;
        protected string domainUrl = "";
        private List<string> disallow = new List<string>();
        protected Queue<string> itemQueue = new Queue<string>(); //itemQueue refers to products/reviews
        protected Queue<string> searchQueue = new Queue<string>();
        protected string currentSite;

        public abstract bool Parse(string siteData);
        public abstract void CrawlPage(string siteData);
        public abstract string GetSiteKey(string url);
        public abstract void CrawlReviewPages(string siteData);
        public abstract string GetProductType(string tempLink);


        public bool StartCycle()
        {
            bool isReview = false;
            bool isItemDone = false;

            if (searchQueue.Count > 0)
            {
                isReview = false;
                currentSite = searchQueue.Dequeue().ToLower();
            }
            else if (itemQueue.Count > 0)
            {
                isReview = true;
                currentSite = itemQueue.Dequeue().ToLower();
            }
            else
            {
                return true;
            }

            if (AmIAllowed(currentSite))
            {
                string siteData = GetSiteData(currentSite);
                if (isReview)
                {
                    isItemDone = Parse(siteData); //Parse information of review/product page.
                }
                else
                {
                    CrawlPage(siteData); //Crawl for new reviews.
                }
            }
            else
            {
                Debug.WriteLine("Robot.txt disallow this site: " + currentSite);
            }

            if (isItemDone) //If a review or product was just "completed" then add it to DB
            {
                //db stuff
            }

            return false;
        }

        public DateTime GetLastAccessTime()
        {
            return visitTimeStamp;
        }

        public void SetLastAccessTime(DateTime newTime)
        {
            visitTimeStamp = newTime;
        }

        private bool AmIAllowed(string URL)
        {
            if (robotsTimeStamp.AddDays(1) <= DateTime.Now)
            {
                getRobotsTxt(domainUrl);
            }

            string wantedSide = URL.Remove(0, domainUrl.Count());

            foreach (string item in disallow)
            {
                if (wantedSide.Contains(item.Trim('\r')))
                {
                    return false;
                }
            }

            return true;
        }

        public string GetSiteData(string siteUrl)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Proxy = null;
            byte[] raw;
            string webData = siteUrl + '\n';

            try
            {
                raw = wc.DownloadData(siteUrl);

                webData += Encoding.UTF8.GetString(raw);
            }
            catch (Exception E)
            {
                Debug.WriteLine("failed to get data from: " + siteUrl);
            }

            return webData;
        }

        private void getRobotsTxt(string domain)
        {
            robotsTimeStamp = DateTime.Now;
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Proxy = null;
            try
            {
                byte[] rawWebData = webClient.DownloadData(domain + "/robots.txt");

                string webData = Encoding.UTF8.GetString(rawWebData);

                string[] webDataLines = webData.Split('\n');

                Boolean concernsMe = false;

                for (int i = 0; i < webDataLines.Count(); i++)
                {
                    if (webDataLines[i] != "" && webDataLines[i].Count() > 11)
                    {
                        if (webDataLines[i].Remove(11, webDataLines[i].Count() - 11).ToLower() == "user-agent:")
                        {
                            concernsMe = false;
                            if (webDataLines[i].Remove(13, webDataLines[i].Count() - 13).ToLower() == "user-agent: *")
                            {
                                concernsMe = true;
                            }
                        }
                    }

                    if (concernsMe && webDataLines[i] != "" && webDataLines[i].Count() > 10)
                    {
                        if (webDataLines[i].Remove(10, webDataLines[i].Count() - 10).ToLower() == "disallow: ")
                        {
                            disallow.Add(webDataLines[i].Remove(0, 10));
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine(domain + "does not contain /robots.txt!");
            }
        }
    }
}