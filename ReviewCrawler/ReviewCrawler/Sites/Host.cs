using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using ReviewCrawler.Products;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Sites.Sub;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using ReviewCrawler.Helpers;

namespace ReviewCrawler.Sites
{
    abstract class Host : HostInterface
    {
        protected DateTime visitTimeStamp = DateTime.Now;
        protected DateTime robotsTimeStamp;
        protected string domainUrl = "";
        private List<string> disallow = new List<string>();

        protected Queue<QueueElement> itemQueue = new Queue<QueueElement>();
            //itemQueue refers to products/reviews depending on the site

        protected Queue<QueueElement> searchQueue = new Queue<QueueElement>();
        protected string currentSite;
        private bool justStarted = true;
        protected DateTime lastSave = DateTime.Now;


        public abstract bool Parse(string siteData, string queueData);
        public abstract void CrawlPage(string siteData, string queueData);
        public abstract void AddItemToDatabase(MySqlConnection connection);

        public bool StartCycle(MySqlConnection connection)
        {
            bool isReview = false;
            bool isItemDone = false;
            QueueElement tempElement;
            //bool startParse = false;
            string queueData = "";

            if (justStarted) //&& this.GetType() !=typeof(SiteEdbPriser) && this.GetType() != typeof(SitePriceRunner))
            {

                LoadCrawlerState(connection);
                justStarted = false;
            }

            if (itemQueue.Count > 0)
            {
                isReview = true;
                tempElement = itemQueue.Dequeue();
                currentSite = tempElement.url;
                queueData = tempElement.data;
            }
            else if (searchQueue.Count > 0)
            {
                isReview = false;
                tempElement = searchQueue.Dequeue();
                currentSite = tempElement.url;
                queueData = tempElement.data;
            }
            else
            {
                return true;
            }

            if (AmIAllowed(currentSite))
            {
                string siteData = GetSiteData(currentSite, isReview, queueData);
                if (isReview)
                {
                    isItemDone = Parse(siteData, queueData); //Parse information of review/product page.
                }
                else
                {
                    CrawlPage(siteData, queueData); //Crawl for new reviews.
                }
            }
            else
            {
                Debug.WriteLine("Robot.txt disallow this site: " + currentSite);
            }

            if (isItemDone) //If a review or product was just "completed" then add it to DB
            {

                connection.Open();
                AddItemToDatabase(connection);
                connection.Close();
                if (!MainWindow.runFast || (DateTime.Now - lastSave).TotalMinutes > 10)
                {
                    SaveCrawlerState(connection);
                    if (!MainWindow.runFast)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void InsertSiteInDB(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO CrawlProgress" +
                                                    "(Site, Queue, date)" +
                                                    "VALUES(@site, @queue, @date)", connection);
            command.Parameters.AddWithValue("@site", this.GetType().ToString());
            command.Parameters.AddWithValue("@queue", "");
            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.ExecuteNonQuery();
        }

        public virtual void SaveCrawlerState(MySqlConnection connection)
        {
            connection.Open();
            if (!DoesSiteExist(connection))
            {
                InsertSiteInDB(connection);
            }
            Queue<QueueElement> tempQueue = new Queue<QueueElement>();

            string queue = "";
            QueueElement tempElement;

            while (searchQueue.Count > 0)
            {
                tempElement = searchQueue.Dequeue();
                queue += (tempElement.url + "%%%##%##" + tempElement.data + "#%&/#");
                tempQueue.Enqueue(tempElement);
            }
            while (tempQueue.Count > 0)
            {
                searchQueue.Enqueue(tempQueue.Dequeue());
            }

            queue += "%%&&##";
            while (itemQueue.Count > 0)
            {
                tempElement = itemQueue.Dequeue();
                queue += (tempElement.url + "%%%##%##" + tempElement.data + "#%&/#");
                tempQueue.Enqueue(tempElement);
            }
            while (tempQueue.Count > 0)
            {
                itemQueue.Enqueue(tempQueue.Dequeue());
            }



            MySqlCommand command =
                new MySqlCommand("UPDATE CrawlProgress SET Queue = @queue, date = @date WHERE site=@site", connection);
            command.Parameters.AddWithValue("@queue", queue);
            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@site", this.GetType().ToString());
            command.ExecuteNonQuery();

            connection.Close();
        }

        public bool DoesSiteExist(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM CrawlProgress WHERE site=@site", connection);
            command.Parameters.AddWithValue("@site", this.GetType().ToString());

            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual void LoadCrawlerState(MySqlConnection connection)
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT * FROM CrawlProgress WHERE Site=@site", connection);
            command.Parameters.AddWithValue("@Site", this.GetType().ToString());

            MySqlDataReader reader = command.ExecuteReader();
            searchQueue.Clear();
            itemQueue.Clear();

            if (reader.Read())
            {
                string queue = (string) reader.GetValue(1);
                string[] queueSplit = queue.Split(new string[] {"%%&&##"}, StringSplitOptions.None);
                string[] sQueue = queueSplit[0].Split(new string[] {"#%&/#"}, StringSplitOptions.None);
                string[] iQueue = queueSplit[1].Split(new string[] {"#%&/#"}, StringSplitOptions.None);

                for (int i = 0; i < sQueue.Length - 1; i++)
                {
                    string[] sSplitTemp = sQueue[i].Split(new string[] {"%%%##%##"}, StringSplitOptions.None);
                    searchQueue.Enqueue(new QueueElement(sSplitTemp[0], sSplitTemp[1]));
                }
                for (int i = 0; i < iQueue.Length - 1; i++)
                {
                    string[] iSplitTemp = iQueue[i].Split(new string[] {"%%%##%##"}, StringSplitOptions.None);
                    itemQueue.Enqueue(new QueueElement(iSplitTemp[0], iSplitTemp[1]));
                }
            }
            reader.Close();
            connection.Close();
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
                GetRobotsTxt(domainUrl);
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

        private string AmazonFixTest(string link)
        {

            if (link.Contains("ref="))
            {
                int index = link.IndexOf("ref=");
                if (index > 0)
                    link = link.Substring(0, index);
            }
            return link;
        }

        public string GetSiteData(string siteUrl, bool isReview, string queueData)
        {
            if (this.GetType() == typeof(SiteAmazon))
            {
                siteUrl = AmazonFixTest(siteUrl);
            }

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Proxy = GetRandomProxy(); // <-------- set to null to disable proxy
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
                if (isReview)
                {
                    itemQueue.Enqueue(new QueueElement(siteUrl, queueData));
                }
                else
                {
                    searchQueue.Enqueue(new QueueElement(siteUrl, queueData));
                }

            }

            return webData;
        }

        private WebProxy GetRandomProxy()
        {
            try
            {
                string filePath = @"Proxy\ProxyList.txt";
                string[] allLines = File.ReadAllLines(filePath);

                //Get random proxy from file
                Random rnd1 = new Random();
                string proxyAddress = allLines[rnd1.Next(allLines.Length)];

                //split into ip and port
                string[] address = proxyAddress.Split(':');
                string ip = address[0];
                int port = int.Parse(address[1]);

                NetworkCredential cred = new NetworkCredential("US221994", "kyMfbgBPpF");
                WebProxy proxy = new WebProxy(proxyAddress, false, null, cred);
                Debug.WriteLine("Using proxy " + proxyAddress);
                return proxy;


            }
            catch (Exception)
            {
                    
                throw;
            }
        }

        private void GetRobotsTxt(string domain)
        {
            robotsTimeStamp = DateTime.Now;
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Proxy = GetRandomProxy(); // <-------- set to null to disable proxy
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