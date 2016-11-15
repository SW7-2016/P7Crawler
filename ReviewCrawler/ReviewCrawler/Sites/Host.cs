using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using ReviewCrawler.Products;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Sites.Sub;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

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
        private bool justStarted = true;
        protected DateTime lastSave = DateTime.Now;

        public abstract bool Parse(string siteData);
        public abstract void CrawlPage(string siteData);
        public abstract void AddItemToDatabase(MySqlConnection connection);

        public bool StartCycle(MySqlConnection connection)
        {
            bool isReview = false;
            bool isItemDone = false;
            //bool startParse = false;

            if (justStarted)
            {
                LoadCrawlerState(connection);
                justStarted = false;
            }

            if (itemQueue.Count > 0)
            {
                isReview = true;
                currentSite = itemQueue.Dequeue().ToLower();
            }
            else if (searchQueue.Count > 0)
            {
                isReview = false;
                currentSite = searchQueue.Dequeue().ToLower();
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


            string queue = "";
            while (searchQueue.Count > 0)
            {
                queue += (searchQueue.Dequeue() + "#%&/#");
            }
            queue += "%%&&##";
            while (itemQueue.Count > 0)
            {
                queue += (itemQueue.Dequeue() + "#%&/#");
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

            
            if (reader.Read())
            {
                string queue = (string)reader.GetValue(1);
                string[] queueSplit = queue.Split(new string[] { "%%&&##" }, StringSplitOptions.None);
                string[] sQueue = queueSplit[0].Split(new string[] { "#%&/#" }, StringSplitOptions.None);
                string[] iQueue = queueSplit[1].Split(new string[] { "#%&/#" }, StringSplitOptions.None);

                for (int i = 0; i < sQueue.Length - 1; i++)
                {
                    searchQueue.Enqueue(sQueue[i]);
                }
                for (int i = 0; i < iQueue.Length - 1; i++)
                {
                    itemQueue.Enqueue(iQueue[i]);
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

        private void GetRobotsTxt(string domain)
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