using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ReviewCrawler.Products;

namespace ReviewCrawler.Sites
{
    abstract class Host : HostInterface
    {
        protected DateTime visitTimeStamp = DateTime.Now;
        protected DateTime robotsTimeStamp;
        protected string domainUrl = "";
        private List<string> disallow = new List<string>();
        protected Queue<KeyValuePair<string, string>> reviewQueue = new Queue<KeyValuePair<string, string>>();
        protected Queue<string> searchQueue = new Queue<string>();
        public List<Product> products = new List<Product>();


        public abstract void Parse(string siteData, string productType);
        public abstract void CrawlPage(string currentSite);

        public bool Crawl()
        {
            string currentSite = "";
            string currentProductType = "";
            bool isReview = false;

            if (searchQueue.Count > 0)
            {
                isReview = false;
                currentSite = searchQueue.Dequeue();
            }
            else if (reviewQueue.Count > 0)
            {
                isReview = true;
                currentProductType = reviewQueue.Peek().Value;
                currentSite = reviewQueue.Dequeue().Key;
            }
            else
            {
                return true;
            }

            if (amIAllowed(currentSite))
            {
                if (isReview)
                {
                    Parse(GetSiteData(currentSite), currentProductType); //Parse information of review/product page.
                }
                else
                {
                    CrawlPage(currentSite); //Crawl for new reviews.
                }
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

        public void SetLastAccessTime(DateTime newTime)
        {
            visitTimeStamp = newTime;
        }

        private bool amIAllowed(string URL)
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

        public List<string> GetReviewLinks(string siteData, string firstTagForReviewLink, string secondTagForReviewLink, string splitTag, bool reverse)
        {
            List<string> reviewLinks = new List<string>();

            string[] splitOnProducts = siteData.Split(new string[] { splitTag }, StringSplitOptions.None);
            string tempReviewLink;

            foreach (var item in splitOnProducts)
            {
                tempReviewLink = GetSearchLinks(item, firstTagForReviewLink, secondTagForReviewLink, reverse);

                if (tempReviewLink != domainUrl)
                {
                    reviewLinks.Add(tempReviewLink);
                }

            }

            return reviewLinks;
        }

        public string GetSearchLinks(string siteData, string firstIdentifier, string secondIdentifier, bool reverse)
        {
            string newSearchLink = "";
            string tempString = "";
            bool linkFound = false;
            bool copyLink = false;
            siteData = siteData.ToLower();

            string[] lines = siteData.Split('\n');

            int firstLineModifier = 0;
            int secondLineModifier = 1;

            if (reverse)
            {
                firstLineModifier = 1;
                secondLineModifier = 0;
            }

            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (lines[i + firstLineModifier].Contains(firstIdentifier) && lines[i + secondLineModifier].Contains(secondIdentifier))
                {
                    for (int j = 0; j < lines[i + secondLineModifier].Length; j++)
                    {
                        if (copyLink == true)
                        {
                            if ((lines[i + secondLineModifier])[j] == '"')
                            {
                                break;
                            }
                            tempString += (lines[i + secondLineModifier])[j];
                        }

                        if ((lines[i + secondLineModifier])[j] == 'h'
                            && (lines[i + secondLineModifier])[j + 1] == 'r'
                            && (lines[i + secondLineModifier])[j + 2] == 'e'
                            && (lines[i + secondLineModifier])[j + 3] == 'f')
                        {
                            linkFound = true;
                        }
                        if (linkFound == true && (lines[i + secondLineModifier])[j] == '"')
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

