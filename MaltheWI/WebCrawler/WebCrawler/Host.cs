using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace WebCrawler
{
    class Host
    {
        public List<string> disallow = new List<string>();
        public List<string> allow = new List<string>();

        public string domain;
        public DateTime timestamp = DateTime.Now;
        public List<string> visitedUrl = new List<string>();

        public Host(string Domain)
        {
            domain = Domain;
            ParseRobot(domain + "/robots.txt");
        }

        public void ParseRobot(string roboUrl)
        {
            bool starUser = false;

            string[] temp = new string[2];


            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Proxy = null;
                byte[] raw = wc.DownloadData(roboUrl);

                string webData = Encoding.UTF8.GetString(raw);

                string[] roboLines = webData.Split('\n');

                for (int i = 0; i < roboLines.Count(); i++)
                {
                    if (roboLines[i] != "" && roboLines[i][0] == ' ')
                    {
                        roboLines[i] = roboLines[i].Remove(0, 1);
                    }

                    if (roboLines[i] != "" && roboLines[i][0] != '#' && starUser == true && roboLines[i].ToLower().Contains("disallow"))
                    {
                        if (roboLines[i].Split(' ').Length < 2)
                        {
                            continue;
                        }
                        temp = roboLines[i].Split(' ');
                        disallow.Add(temp[1]);
                    }
                    else if (roboLines[i] != "" && roboLines[i][0] != '#' && starUser == true && roboLines[i].ToLower().Contains("allow"))
                    {
                        if (roboLines[i].Split(' ').Length < 2)
                        {
                            continue;
                        }
                        temp = roboLines[i].Split(' ');
                        allow.Add(temp[1]);
                    }
                    else if (roboLines[i] != "" && roboLines[i][0] != '#' && starUser == true && roboLines[i].ToLower().Contains("user-agent"))
                    {
                        starUser = false;
                    }



                    if (roboLines[i] != "" && roboLines[i][0] != '#' && roboLines[i].ToLower().Replace(" ", "") == "user-agent:*")
                    {
                        starUser = true;
                    }
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(roboUrl);
            }

        }
    }
}
