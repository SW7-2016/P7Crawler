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
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using ReviewCrawler.Helpers;

namespace ReviewCrawler.Sites
{
    abstract class Host : HostInterface
    {
        private DateTime visitTimeStamp = DateTime.Now; //Last visit to this host
        private DateTime robotsTimeStamp; //Last time robots.txt was checked
        protected string domainUrl = "";
        private List<string> disallow = new List<string>();
        protected Queue<QueueElement> itemQueue = new Queue<QueueElement>();//itemQueue refers to products/reviews depending on the site
        protected Queue<QueueElement> searchQueue = new Queue<QueueElement>();//Contains intermediate links between reviews/products
        protected string currentSite; //url of the currently selected site
        private bool justStarted = true; //Used to check if this is first runthrough of  
        protected DateTime lastSave = DateTime.Now; //last time this hosts state was saved
        
        public abstract bool Parse(string siteData, string queueData);
        public abstract void Crawl(string siteData, string queueData);
        public abstract void AddItemToDatabase(MySqlConnection connection);

        //Called from crawler.cs, represents one crawl/parse cycle
        public bool CrawlParseCycle(MySqlConnection connection)
        {
            bool isContentSite; //Does the current subSite need to be parsed? else crawl
            bool isItemDone = false;
            QueueElement subSite; //Represents an item from searchqueue or itemqueue (url + extra data field)
            string currentSiteData;

            if (justStarted) //is this the first crawlparse cycle this session for the current host
            {
                LoadCrawlerState(connection); //Loads last saved state of this host from db
                justStarted = false;
            }

            subSite = GetNextSubSite(out isContentSite); //this method also sets "isContentSite" to true or false
            if (subSite != null)
            {
                currentSite = subSite.url;
                currentSiteData = subSite.data;
            }
            else
            {
                return true; //if no more items are left in itemqueue and searchqueue 
            }
            
            if (AmIAllowed(currentSite)) //checks the list extracted from robot.txt if the current link is dissallowed by the host
            {
                string siteData = GetSiteData(currentSite, isContentSite, currentSiteData); //Gets data from site
                if (siteData.Length < 50)  ///REEEEEEEEEMOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOVEEEEE
                {
                    return false;
                }                          ///REEEEEEEEEMOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOVEEEEE
                if (isContentSite) //Does sitedata need to be parsed?
                {
                    isItemDone = Parse(siteData, currentSiteData); //Parse information of review/product page. Returns true if the current product/review has been completed
                }
                else //sitedata needs to be crawled
                {
                    Crawl(siteData, currentSiteData); //Crawl for new reviews.
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
                //if stop button has been pressed or the host state have not been saved for more than 10 minutes
                if (!MainWindow.ContinueCrawling || (DateTime.Now - lastSave).TotalMinutes > 10)
                {
                    SaveCrawlerState(connection);
                    //Stop crawler from requeuing host if stop button has been pressed
                    if (!MainWindow.ContinueCrawling)
                    {
                        return true; //Returned to isHostDone in host.cs
                    }
                }
            }
            return false; //Host still has more work to do
        }

        //Gets next subSite to crawl/parse
        private QueueElement GetNextSubSite(out bool isContentSite)
        {
            if (itemQueue.Count > 0)
            {
                isContentSite = true;
                return itemQueue.Dequeue();
            }
            if (searchQueue.Count > 0)
            {
                isContentSite = false;
                return searchQueue.Dequeue();
            }

            isContentSite = false;
            return null;
        }

        //Used when saving the host state into db
        private void InsertSiteInDB(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO CrawlProgress" +
                                                    "(Site, Queue, date)" +
                                                    "VALUES(@site, @queue, @date)", connection);
            command.Parameters.AddWithValue("@site", this.GetType().ToString());
            command.Parameters.AddWithValue("@queue", "");
            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.ExecuteNonQuery();
        }

        //Used when saving the host state into db
        private void SaveCrawlerState(MySqlConnection connection)
        {
            connection.Open();
            if (!DoesSiteExist(connection)) //if previous site save does not exist in db 
            {
                InsertSiteInDB(connection); //add new row for current site
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

            //Updates save in db
            MySqlCommand command =
                new MySqlCommand("UPDATE CrawlProgress SET Queue = @queue, date = @date WHERE site=@site", connection);
            command.Parameters.AddWithValue("@queue", queue);
            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@site", this.GetType().ToString());
            command.ExecuteNonQuery();

            connection.Close();
        }

        //checks if a site already exists in CrawlProgress table of db
        private bool DoesSiteExist(MySqlConnection connection)
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
        //used to load host state from db
        private void LoadCrawlerState(MySqlConnection connection)
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

        //Gets last access time of current host
        public DateTime GetLastAccessTime()
        {
            return visitTimeStamp;
        }
        //sets a new access time for current host
        public void SetLastAccessTime(DateTime newTime)
        {
            visitTimeStamp = newTime;
        }

        //Checks an url against robot.txt's dissallow list and returns whether it the url is part of the list
        private bool AmIAllowed(string URL)
        {
            if (robotsTimeStamp.AddDays(1) <= DateTime.Now) //if more than a day has passed since last robot.txt check
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

        //Gets content of a website and returns it as a string
        public string GetSiteData(string siteUrl, bool isContentSite, string queueData)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Proxy = GetRandomProxy(); // <-------- set to null to disable proxy
            byte[] raw;
            string webData = siteUrl + '\n';
            try
            {
                raw = wc.DownloadData(siteUrl);

                webData += Encoding.UTF8.GetString(raw);
                if (webData.Contains("<title dir=\"ltr\">Robot Check</title>")) ///REEEEEEEEEMOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOVEEEEE
                {
                    webData = "";
                    if (isContentSite)
                    {
                        itemQueue.Enqueue(new QueueElement(siteUrl, queueData));
                    }
                    else
                    {
                        searchQueue.Enqueue(new QueueElement(siteUrl, queueData));
                    }
                }                                                               ///REEEEEEEEEMOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOVEEEEE
            }
            catch (Exception E)
            {
                Debug.WriteLine("failed to get data from: " + siteUrl);
                if (isContentSite) //requeues the item since this usually happens because of a connection error
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
        //updates dissallow list from robot.txt
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
                disallow.Clear();

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
                Debug.WriteLine(domain + " does not contain /robots.txt!");
            }
        }
    }
}