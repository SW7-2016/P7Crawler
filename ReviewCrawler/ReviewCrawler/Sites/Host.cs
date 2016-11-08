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
        protected Queue<string> reviewQueue = new Queue<string>();
        protected Queue<string> searchQueue = new Queue<string>();
        protected string currentSite;

        public abstract void Parse(string siteData);
        public abstract void CrawlPage(string siteData);
        public abstract string GetSiteKey(string url);
        public abstract void CrawlReviewPages(string siteData);

        public bool StartCycle()
        {
            bool isReview = false;

            if (searchQueue.Count > 0)
            {
                isReview = false;
                currentSite = searchQueue.Dequeue().ToLower();
            }
            else if (reviewQueue.Count > 0)
            {
                isReview = true;
                currentSite = reviewQueue.Dequeue().ToLower();
            }
            else
            {
                return true;
            }

            if (amIAllowed(currentSite))
            {
                string siteData = GetSiteData(currentSite);
                if (isReview)
                {
                    CrawlReviewPages(siteData);
                    Parse(siteData); //Parse information of review/product page.
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

        public string removeTags(string siteData)
        {

            string tempString = "";

            foreach (Match item in Regex.Matches(siteData, "((<p>|<p style|<p align=\"left\">).*?(<\\/p>))+", RegexOptions.IgnoreCase))
            {
                tempString += item + "\n";
            }

            Regex newlineAdd = new Regex("<br />", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex regexHtml = new Regex("(<.*?>)+", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex apostropheRemover = new Regex("\\&rsquo\\;", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex garbageRemover = new Regex("\\&nbsp\\;", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            tempString = newlineAdd.Replace(tempString, "\n");
            tempString = regexHtml.Replace(tempString, "");
            tempString = apostropheRemover.Replace(tempString, "");
            tempString = garbageRemover.Replace(tempString, " ");

            tempString += "\n";     

            return tempString;
        }

        public abstract string GetProductType(string tempLink);

    }
}

