using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Products.Reviews;
using System.Text.RegularExpressions;
using System.IO;

namespace ReviewCrawler.Sites.Sub
{
    class SiteGuru3d : Host
    {
        double maxRating = 5;

        public SiteGuru3d()
        {
            domainUrl = "http://www.guru3d.com/";
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/videocards.html");
            //searchQueue.Enqueue("http://www.guru3d.com/articles-categories/processors.html");
            //searchQueue.Enqueue("http://www.guru3d.com/articles-categories/soundcards-and-speakers.html");
            //searchQueue.Enqueue("http://www.guru3d.com/articles-categories/mainboards.html");
            //searchQueue.Enqueue("http://www.guru3d.com/articles-categories/memory-(ddr2%7Cddr3)-and-storage-(hdd%7Cssd).html");
            //searchQueue.Enqueue("http://www.guru3d.com/articles-categories/pc-cases-and-modding.html");
            //searchQueue.Enqueue("http://www.guru3d.com/articles-categories/psu-power-supply-units.html");
            //searchQueue.Enqueue("http://www.guru3d.com/articles-categories/cooling.html");
        }

        public override void CrawlPage(string siteData)
        {
            string tempLink = "";
            string tempSiteKey = "";
            List<string> tempReviewLinks;

            tempLink = GetSearchLinks(siteData, "pagelinkselected", "pagelink", false); //Returns domainUrl if no link is found
            if (tempLink != domainUrl)
            {
                searchQueue.Enqueue(tempLink);
            }
     
            tempReviewLinks = GetReviewLinks(siteData, "<br />", "<a href=\"articles-pages", "<div class=\"content\">", true);
            foreach (var item in tempReviewLinks)
            {
                tempSiteKey = GetSiteKey(item);
                //if (reviewQueue.Count < 4)
                //{
                    reviewQueue.Enqueue(item);
                //}

                if (!Crawler.reviews.ContainsKey(tempSiteKey))
                {
                    Crawler.reviews.Add(tempSiteKey, new Review(item, GetProductType(currentSite)));
                }



            }

        }

        public override void CrawlReviewPages(string siteData)
        {
            string tempLink = "";
            
            if (!currentSite.Contains("articles-summary"))
            {
                tempLink = GetSearchLinks(siteData, "pagelinkselected", "pagelink", false); //Returns domainUrl if no link is found


                if (tempLink != domainUrl)
                {
                    reviewQueue.Enqueue(tempLink);
                }
                else
                {
                    //GetSiteKey just so happens to give the wanted output, even though it is not used as a key here
                    reviewQueue.Enqueue(GetSiteKey(currentSite.Replace("articles-pages", "articles-summary")));
                }
            }

        }

        /*
        public override void CrawlReviewPages(string siteData)
        {
            string tempLink = "";
            string[] tempLines = siteData.Split('\n');

            if (!tempLines[0].Contains("articles-summary"))
            {
                tempLink = GetSearchLinks(siteData, "pagelinkselected", "pagelink", false); //Returns domainUrl if no link is found


                if (tempLink != domainUrl)
                {
                    reviewQueue.Enqueue(tempLink);
                }
                else
                {
                    tempLines[0] = GetSiteKey(tempLines[0].Replace("articles-pages", "articles-summary"));
                    //GetSiteKey just so happens to give the wanted output, even though it is not used as a key here


                    reviewQueue.Enqueue(tempLines[0]);
                }
            }

        }*/


        public override string GetSiteKey(string url)
        {
            for (int i = url.Length - 1; i > 0; i--)
            {
                if (url[i] == ',')
                {
                    url = url.Remove(i, url.Length - i);
                    break;
                } 
            }

            return url;
        }

        public override string GetProductType(string tempLink)
        {
            if (tempLink.Contains("articles-categories/videocards"))
            {
                return "GPU";
            }
            else if (tempLink.Contains("articles-categories/processors"))
            {
                return "CPU";
            }
            else if (tempLink.Contains("articles-categories/soundcards-and-speakers"))
            {
                return "SoundCard";
            }
            else if (tempLink.Contains("articles-categories/mainboards"))
            {
                return "Motherboard";
            }
            else if (tempLink.Contains("articles-categories") && tempLink.Contains("memory") && tempLink.Contains("storage"))
            {
                return "RAM/HDD";
            }
            else if (tempLink.Contains("articles-categories/pc-cases-and-modding"))
            {
                return "Chassis";
            }
            else if (tempLink.Contains("articles-categories/psu-power-supply-units"))
            {
                return "PSU";
            }
            else if (tempLink.Contains("articles-categories/cooling"))
            {
                return "Cooling";
            }
            else
            {
                Debug.WriteLine("couldnt determine product type - GetProductType, guru3d");
            }

            return "";
        }

        public override void Parse(string siteData)
        {
            string siteContentParsed = removeTags(siteData);
            string siteKey;
            string[] dataSplit = siteContentParsed.Split('\n');

            if (!currentSite.Contains("articles-summary"))
            {
                siteKey = GetSiteKey(currentSite);
                siteContentParsed = removeTags(siteData);
                dataSplit = siteContentParsed.Split('\n');

                if (Crawler.reviews.ContainsKey(siteKey))
                {
                    Crawler.reviews[siteKey].content += siteContentParsed;

                    if (currentSite.Contains(",1.html"))
                    {
                        Crawler.reviews[siteKey].title = dataSplit[0];
                        Crawler.reviews[siteKey].productRating = GetRating(siteData);
                        Crawler.reviews[siteKey].maxRating = maxRating;
                        Crawler.reviews[siteKey].crawlDate = DateTime.Now;
                        Crawler.reviews[siteKey].reviewDate = GetReviewDate(siteData);
                    }
                }
            }
            else
            {
                siteKey = GetSiteKey(currentSite.Replace("articles-summary", "articles-pages"));

                if (Crawler.reviews.ContainsKey(siteKey))
                {
                    Crawler.reviews[siteKey].comments = GetReviewComments(siteData);
                }
            }

            
        }

        public List<ReviewComment> GetReviewComments(string siteData)
        {
            List<ReviewComment> commentResults = new List<ReviewComment>();
            Regex regexTags = new Regex("(<.*?>)+", RegexOptions.Singleline);
            Regex regexDateRemove = new Regex("(<strong>Posted on:(.*?)</strong>)", RegexOptions.Singleline);
            Regex regexQuote = new Regex("(<table(.*?)</table>)", RegexOptions.Singleline);
            Regex regexQuoteBlock = new Regex("(<div class=\"quoteblock(.*?)</div>)", RegexOptions.Singleline);
            string tempComment;

            string[] commentSplit = siteData.Split(new string[] { "<div class=\"comments\">" }, StringSplitOptions.None);

            for (int i = 0; i < commentSplit.Length; i++)
            {
                
                tempComment = Regex.Match(commentSplit[i], "(<strong>Posted on:.*?<!-- Template: articles)", RegexOptions.Singleline).Value;
                if (tempComment != "")
                {
                    tempComment += '>';
                }
                

                tempComment = regexDateRemove.Replace(tempComment, "");
                tempComment = regexTags.Replace(tempComment, "");
                tempComment = regexQuote.Replace(tempComment, "");
                tempComment = regexQuoteBlock.Replace(tempComment, "");

                if (tempComment != "")
                {
                    commentResults.Add(new ReviewComment(tempComment, 0));
                }
            }
            return commentResults;
        }

        public DateTime GetReviewDate(string siteData)
        {
            DateTime date; 
            string temp = Regex.Match(siteData, "<span itemprop=\"dtreviewed\">.*?</span>").Value;
            

            temp = temp.Replace("<span itemprop=\"dtreviewed\">", "");
            temp = temp.Replace("</span>", "");

            string[] tempDate = temp.Split('/');

            tempDate[2] = tempDate[2].Substring(0, 4);

            if ((tempDate[0])[0] == '0')
            {
                tempDate[0] = (tempDate[0])[1].ToString();
            }

            int day = int.Parse(tempDate[1]);
            int month = int.Parse(tempDate[0]);
            int year = int.Parse(tempDate[2]);

            date = new DateTime(year, month, day);

            return date;
        }

        public double GetRating(string siteData)
        {
            string strValue = "";
            string temp = Regex.Match(siteData, "<meta itemprop=\"value\" content=\".*?\"/>").Value;

            for (int i = 0; i < temp.Length - 1; i++)
            {
                if (char.IsNumber(temp[i]))
                {
                    strValue += temp[i];
                }
            }
            if (strValue != "")
            {
                return double.Parse(strValue);
            }
            else
            {
                return -1;
            }
            
        }

        /*
        public override void CrawlPage(string currentSite)
        {
            string siteData = GetSiteData(currentSite);
            string tempLink = "";
            List<string> tempReviewLinks;
            string tempProductType;

            tempLink = GetSearchLinks(siteData, "pagelinkselected", "pagelink", false); //Returns domainUrl if no link is found
            if (tempLink != domainUrl)
            {
                searchQueue.Enqueue(tempLink);
            }
            tempProductType = GetProductType(tempLink);
            tempReviewLinks = GetReviewLinks(siteData, "<br />", "<a href=\"articles-pages", "<div class=\"content\">", true);
            foreach (var item in tempReviewLinks)
            {
                reviewQueue.Enqueue(new KeyValuePair<string, string>(item, tempProductType));
            }
        }*/

    }
}
