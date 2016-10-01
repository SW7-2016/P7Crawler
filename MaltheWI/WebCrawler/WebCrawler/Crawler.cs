using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WebCrawler
{
    class Crawler
    {
        

        public static Queue<Site> frontQueue = new Queue<Site>();
        public static List<Host> backList = new List<Host>();
        public static List<string> checkedsites = new List<string>();
        public static int hostId = 0;
        public static int siteId = 0;
        public static List<Site> crawlData = new List<Site>();

        public long GetHashCodeInt64(string input)
        {
            var s1 = input.Substring(0, input.Length / 2);
            var s2 = input.Substring(input.Length / 2);

            var x = ((long)s1.GetHashCode()) << 0x20 | s2.GetHashCode();

            return x;
        }

        public double NearDuplicate(string str1, string str2)
        {
            str1 = str1.Replace(",", "");
            str2 = str2.Replace(",", "");
            str1 = str1.Replace(".", "");
            str2 = str2.Replace(".", "");

            string[] tempArray1 = str1.Split(' ');

            string[] tempArray2 = str2.Split(' ');

            string[] shingles1 = new string[tempArray1.Count() - 2];

            string[] shingles2 = new string[tempArray2.Count() - 2];

            for (int i = 0; i < tempArray1.Count() - 2; i++)
            {
                shingles1[i] = tempArray1[i] + tempArray1[i + 1] + tempArray1[i + 2];
            }
            for (int i = 0; i < tempArray2.Count() - 2; i++)
            {
                shingles2[i] = tempArray2[i] + tempArray2[i + 1] + tempArray2[i + 2];
            }


            int[] hashedshingles1 = new int[shingles1.Count()];

            for (int i = 0; i < shingles1.Count(); i++)
            {
                hashedshingles1[i] = shingles1[i].GetHashCode();
            }

            int[] hashedshingles2 = new int[shingles2.Count()];

            for (int i = 0; i < shingles2.Count(); i++)
            {
                hashedshingles2[i] = shingles2[i].GetHashCode();
            }

            long[] hashedshingles3 = new long[shingles1.Count()];

            for (int i = 0; i < shingles1.Count(); i++)
            {
                hashedshingles3[i] = GetHashCodeInt64(shingles1[i]);
            }

            long[] hashedshingles4 = new long[shingles2.Count()];

            for (int i = 0; i < shingles2.Count(); i++)
            {
                hashedshingles4[i] = GetHashCodeInt64(shingles2[i]);
            }

            int overlap = 0;
            int union = 0;

            long[] minhash1 = new long[2];

            minhash1[0] = hashedshingles1.Min();
            minhash1[1] = hashedshingles3.Min();

            long[] minhash2 = new long[2];

            minhash2[0] = hashedshingles2.Min();
            minhash2[1] = hashedshingles4.Min();

            union = 2;

            for (int i = 0; i < minhash1.Count(); i++)
            {
                if (minhash1[i] == minhash2[i])
                {
                    overlap++;
                }

            }

            return (double)overlap / (double)union;
        }

        public  double NearDuplicate2(string str1, string str2)
        {

            str1 = str1.Replace(",", "");
            str2 = str2.Replace(",", "");
            str1 = str1.Replace(".", "");
            str2 = str2.Replace(".", "");

            string[] tempArray1 = str1.Split(' ');

            string[] tempArray2 = str2.Split(' ');

            string[] shingles1 = new string[tempArray1.Count() - 2];

            string[] shingles2 = new string[tempArray2.Count() - 2];

            for (int i = 0; i < tempArray1.Count() - 2; i++)
            {
                shingles1[i] = tempArray1[i] + tempArray1[i + 1] + tempArray1[i + 2];
            }
            for (int i = 0; i < tempArray2.Count() - 2; i++)
            {
                shingles2[i] = tempArray2[i] + tempArray2[i + 1] + tempArray2[i + 2];
            }

            int overlap = 0;
            int union = 0;

            union = (shingles1.Union<string>(shingles2)).Count();

            foreach (var item in shingles1)
            {
                foreach (var item2 in shingles2)
                {
                    if (item == item2)
                    {
                        overlap++;

                    }

                }
            }

            return (double)overlap / (double)union;
        }

        /*public void Crawler()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] raw = wc.DownloadData("http://www.gosugamers.net/counterstrike/gosubet?r-page=" + page.ToString());

            string webData = Encoding.UTF8.GetString(raw);
        }*/

        public void startCrawl(List<string> seed)
        {
            bool running = true;
            Site currentSite;

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Proxy = null;
            byte[] raw;
            string webData;

            foreach (var item in seed)
            {
                frontQueue.Enqueue(new Site(item, ""));
            }
            int tt = 0;
            while (running)
            {
                if (frontQueue.Count == 1)
                {
                    running = false;
                }

                currentSite = frontQueue.Dequeue();

                Debug.WriteLine(currentSite.url);

                if (IsDuplicate(currentSite) || RestrictedSite(currentSite))
                {
                    continue;
                }
   
                if ((DateTime.Now - currentSite.parent.timestamp).TotalSeconds < 2)
                {
                    frontQueue.Enqueue(currentSite);
                    continue;
                }

                try
                {
                    raw = wc.DownloadData(currentSite.url);

                    webData = Encoding.UTF8.GetString(raw);

                    currentSite.content = ParseContent(webData);
                }
                catch(Exception E)
                {
                    Debug.WriteLine(currentSite.url);
                    continue;
                }

                



                crawlData.Add(currentSite);


                if (frontQueue.Count > 0)
                {
                    running = true;
                }

                if (tt > 50)
                {
                    running = false;
                }
                tt++;
                Debug.WriteLine(tt);
                checkedsites.Add(currentSite.url);
            }
            

        }

        

        public string ParseContent(string data)
        {
            GetLinks(data);
            return GetBody(data);

        }

        public string GetBody(string data)
        {
            bool save = false;
            string body = "";

            body = data.ToLower();
            /*
            for (int i = 0; i < data.Length; i++)
            {

                if (save == true && data[i] == '<')
                {
                    save = false;
                }

                if (save == true)
                {
                    body += data[i];
                }

                if (save == false && data[i] == '>')
                {
                    save = true;
                }
            } */
            //body = body.Replace(" ", "\n");

            Regex regexHtlm = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);
            Regex regexJava = new Regex("({.*?}\\s*)+", RegexOptions.Singleline);
            Regex regexSquare = new Regex("(\\[.*?\\]\\s*)+", RegexOptions.Singleline);
            Regex regexScript = new Regex("(<script.*?</script>\\s*)+", RegexOptions.Singleline);
            Regex regexQuotation = new Regex("\"", RegexOptions.Singleline);
            Regex regex1 = new Regex("\\)", RegexOptions.Singleline);
            Regex regex2 = new Regex("\\(", RegexOptions.Singleline);
            Regex regex3 = new Regex("\\-", RegexOptions.Singleline);
            Regex regex4 = new Regex("\\.", RegexOptions.Singleline);
            Regex regex5 = new Regex("\\,", RegexOptions.Singleline);
            Regex regex6 = new Regex("\\!", RegexOptions.Singleline);
            Regex regex7 = new Regex("\\?", RegexOptions.Singleline);
            Regex regex8 = new Regex("\\:", RegexOptions.Singleline);
            Regex regex9 = new Regex("\\;", RegexOptions.Singleline);
            body = regexJava.Replace(body, " ");
            body = regexHtlm.Replace(body, " ");
            body = regexSquare.Replace(body, " ");
            body = regexScript.Replace(body, " ");
            body = regex1.Replace(body, " ");
            body = regex2.Replace(body, " ");
            body = regex3.Replace(body, " ");
            body = regex4.Replace(body, " ");
            body = regex5.Replace(body, " ");
            body = regex6.Replace(body, " ");
            body = regex7.Replace(body, " ");
            body = regex8.Replace(body, " ");
            body = regex9.Replace(body, " ");

            return body;
        }

        public void GetLinks(string data)
        {
            List<string> tempLinks = new List<string>();
            string tempString = "";

            string[] contentArray = data.Split(' ');

            for (int i = 0; i < contentArray.Length; i++)
            {
                if (contentArray[i].ToLower().Contains("http"))
                {
                    tempLinks.Add(contentArray[i]);
                }
            }

            bool isLink = false;

            for (int i = 0; i < tempLinks.Count; i++)
            {
                isLink = false;
                tempString = "";
                for (int j = 0; j < tempLinks[i].Length; j++)
                {
                    if (isLink == true && (tempLinks[i])[j] == '"')
                    {
                        Uri uriResult;
                        if (Uri.TryCreate(tempString, UriKind.Absolute, out uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                            && !tempString.ToLower().EndsWith("png")
                            && !tempString.ToLower().EndsWith("jpg")
                            && !tempString.ToLower().EndsWith("jpeg")
                            && !tempString.ToLower().EndsWith("mp3")
                            && !tempString.ToLower().EndsWith("mp4")
                            && !tempString.ToLower().EndsWith("mpeg")
                            && !tempString.ToLower().Contains(".gif")
                            && !tempString.ToLower().Contains("giphy")
                            && !tempString.ToLower().Contains("imgur"))
                        {
                            frontQueue.Enqueue(new Site(tempString, ""));
                        }
                        
                        break;
                    }
                    if (isLink == true)
                    {
                        tempString += (tempLinks[i])[j];
                        continue;
                    }

                    if ((tempLinks[i])[j] == '"' && (tempLinks[i]).Length > j+4)
                    {
                        if ((tempLinks[i])[j+1] == 'h' || (tempLinks[i])[j + 1] == 'H' 
                            && (tempLinks[i])[j + 2] == 't' || (tempLinks[i])[j + 2] == 'T' 
                            && (tempLinks[i])[j + 3] == 't' || (tempLinks[i])[j + 3] == 'T' 
                            && (tempLinks[i])[j + 4] == 'p' || (tempLinks[i])[j + 4] == 'P')
                        {
                            isLink = true;   
                        }
                    }
                }
            }

        }

        

        public bool RestrictedSite(Site currentSite)
        {
            bool disallowed = false;

            for (int i = 0; i < currentSite.parent.disallow.Count; i++)
            {
                if (currentSite.url.Contains(currentSite.parent.disallow[i]))
                {
                    for (int j = 0; j < currentSite.parent.allow.Count; j++)
                    {
                        if (currentSite.url.Contains(currentSite.parent.allow[j]))
                        {
                            break;
                        }
                    }
                    
                    disallowed = true;
                    break;
                }
            }

            return disallowed;
        }

        public bool IsDuplicate(Site currentSite)
        {
            bool duplicate = false;

            for (int i = 0; i < currentSite.parent.visitedUrl.Count; i++)
            {
                if (currentSite.url == currentSite.parent.visitedUrl[i])
                {
                    duplicate = true;
                    break;
                }
            }

            return duplicate;
        }
    }

}
