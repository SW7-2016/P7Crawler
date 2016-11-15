using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI.objects
{
    public class Host
    {
        public string domain;
        public bool robots = false;
        List<string>[] rules;
        public DateTime timeStamp = DateTime.Now;

        public Host(string Domain)
        {
            domain = Domain;
        }

        public void getRobotsTxt()
        {
            robots = true;
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Proxy = null;
            try
            {
                byte[] rawWebData = webClient.DownloadData(domain + "/robots.txt");

                string webData = Encoding.UTF8.GetString(rawWebData);

                string[] webDataLines = webData.Split('\n');

                Boolean concernsMe = false;

                List<string>[] result = { new List<string>(), new List<string>() };

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
                            result[0].Add(webDataLines[i].Remove(0, 10));
                        }
                    }

                    if (concernsMe && webDataLines[i] != "" && webDataLines[i].Count() > 7)
                    {
                        if (webDataLines[i].Remove(7, webDataLines[i].Count() - 7).ToLower() == "allow: ")
                        {
                            result[1].Add(webDataLines[i].Remove(0, 7));
                        }
                    }
                }

                rules = result;
            }
            catch
            {
                Console.WriteLine(domain + "does not contain /robots.txt!");
                List<string>[] result = { new List<string>(), new List<string>() };
                rules = result;
            }
        }

        public bool amIAllowed(string URL)
        {
            string wantedSide = URL.Remove(0, domain.Count());

            foreach (string item in rules[0])
            {
                if (wantedSide.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
