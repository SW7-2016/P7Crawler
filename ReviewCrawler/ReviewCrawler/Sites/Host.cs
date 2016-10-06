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
        protected DateTime visitTimeStamp = DateTime.Now;
        protected DateTime robotsTimeStamp;
        protected string domainUrl = "";
        private List<string> disallow = new List<string>();
        protected Queue<string> reviewQueue = new Queue<string>();
        protected Queue<string> searchQueue = new Queue<string>();


        public abstract void Parse(string siteData);
        public abstract void CrawlPage(string currentSite, bool isReview);

        public bool Crawl()
        {

            //Checks if there is more content to crawl on this host
            if (reviewQueue.Count < 1 && searchQueue.Count < 1)
            {
                return true;
            }
            

            string currentSite = "";
            bool isReview = false;

            if (searchQueue.Count > 0)
            {
                isReview = false;
                currentSite = searchQueue.Dequeue();
            }
            else if (reviewQueue.Count > 0)
            {
                isReview = true;
                currentSite = reviewQueue.Dequeue();
            }

            if (amIAllowed(currentSite))
            {
                CrawlPage(currentSite, isReview); //Handles the site specific crawling, is overwritten in subclasses
            }
            else
            {
                Debug.WriteLine("Robot.txt disallow this site: " + currentSite);
            }

            return false;


        }

        public DateTime GetLastAccessTime()
        {
            return visitTimeStamp;
        }

        private bool amIAllowed(string URL)
        {
            if (robotsTimeStamp == null || robotsTimeStamp.AddDays(1) >= DateTime.Now)
            {
                getRobotsTxt(domainUrl);
            }

            string wantedSide = URL.Remove(0, domainUrl.Count());

            foreach (string item in disallow)
            {
                if (wantedSide.Contains(item))
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

        public List<string> GetReviewLinks(string siteData, string firstTagForReviewLink, string secondTagForReviewLink, string splitTag)
        {
            List<string> reviewLinks = new List<string>();

            string[] splitOnProducts = siteData.Split(new string[] { splitTag }, StringSplitOptions.None);
            string tempReviewLink;

            foreach (var item in splitOnProducts)
            {
                tempReviewLink = GetSearchLinks(item, firstTagForReviewLink, secondTagForReviewLink);

                if (tempReviewLink != domainUrl)
                {
                    reviewLinks.Add(tempReviewLink);
                }

            }

            return reviewLinks;
        }

        public string GetSearchLinks(string siteData, string firstIdentifier, string secondIdentifier)
        {
            string newSearchLink = "";
            string tempString = "";
            bool linkFound = false;
            bool copyLink = false;
            siteData = siteData.ToLower();

            string[] lines = siteData.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(firstIdentifier) && lines[i + 1].Contains(secondIdentifier))
                {
                    for (int j = 0; j < lines[i + 1].Length; j++)
                    {
                        if (copyLink == true)
                        {
                            if ((lines[i + 1])[j] == '"')
                            {
                                break;
                            }
                            tempString += (lines[i + 1])[j];
                        }

                        if ((lines[i + 1])[j] == 'h'
                            && (lines[i + 1])[j + 1] == 'r'
                            && (lines[i + 1])[j + 2] == 'e'
                            && (lines[i + 1])[j + 3] == 'f')
                        {
                            linkFound = true;
                        }
                        if (linkFound == true && (lines[i + 1])[j] == '"')
                        {
                            copyLink = true;
                        }

                    }
                }
                if (copyLink == true)
                {
                    break;
                }
            }

            newSearchLink = (domainUrl + tempString);

            return newSearchLink;
        }
    }
}

