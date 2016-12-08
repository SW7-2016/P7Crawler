using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Products.Reviews;
using System.Text.RegularExpressions;
using System.IO;
using MySql.Data.MySqlClient;
using ReviewCrawler.Helpers;

namespace ReviewCrawler.Sites.Sub
{
    class SiteGuru3d : ReviewSite
    {
        private const double maxRating = 5;

        public SiteGuru3d()
        {
            domainUrl = "http://www.guru3d.com/";
            
            searchQueue.Enqueue(new QueueElement("http://www.guru3d.com/articles-categories/processors.html", "")); //Finished
            searchQueue.Enqueue(new QueueElement("http://www.guru3d.com/articles-categories/mainboards.html", "")); //finished
            searchQueue.Enqueue(new QueueElement("http://www.guru3d.com/articles-categories/memory-(ddr2%7Cddr3)-and-storage-(hdd%7Cssd).html", ""));   //Finished
            searchQueue.Enqueue(new QueueElement("http://www.guru3d.com/articles-categories/pc-cases-and-modding.html", ""));        //Finished
            searchQueue.Enqueue(new QueueElement("http://www.guru3d.com/articles-categories/psu-power-supply-units.html", ""));  //finished
            searchQueue.Enqueue(new QueueElement("http://www.guru3d.com/articles-categories/videocards.html", "")); //finished
        }

        public override void Crawl(string siteData, string sQueueData)
        {
            string nextPageLink = "";
            string[] articleLinks;
            string tempProductType = "unknown";

            if (currentSite.Contains("articles-categories")) // if on category page
            {
                //Gets link to next category page
                nextPageLink = regexMatch(siteData, "<span class = \"pagelinkselected\">", "</a></span>");
                nextPageLink = regexMatch(nextPageLink, "<span class = \"pagelink\"><a href=\"", "\">");

                tempProductType = GetProductType(currentSite);

                if (nextPageLink != "")
                {
                    searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink, tempProductType));
                }
            }

            //Gets review links from category page
            articleLinks = regexMatches(siteData, "<a href=\"", "\">Read article</a>");

            foreach (string link in articleLinks)
            {
                searchQueue.Enqueue(new QueueElement(domainUrl + link, tempProductType));
            }
            //Adds all pages of review to itemQueue
            CrawlReviewPages(siteData, sQueueData);
        }

        //Adds all pages of review to itemQueue
        private void CrawlReviewPages(string siteData, string productType)
        {
            string tempLink = "";
            int totalPages = 0;

            if (currentSite.Contains(",1.html")) //Is this page 1 of a review?
            {
                totalPages = GetReviewTotalPages(siteData);

                for (int i = 1; i <= totalPages; i++)
                {
                    tempLink = currentSite.Replace("1.html", i + ".html");
                    itemQueue.Enqueue(new QueueElement(tempLink, productType));
                }
                itemQueue.Enqueue(new QueueElement(ReformatUrl(currentSite.Replace("articles-pages", "articles-summary")), ""));
            }
        }

        //Gets total amount of pages for a review
        private int GetReviewTotalPages(string siteData)
        {
            string siteNumber = Regex.Match(siteData, "<span class=\"pagelink\">.*? pages</span>").Value;
            siteNumber = siteNumber.Replace("<span class=\"pagelink\">", "");
            siteNumber = siteNumber.Replace(" pages</span>", "").Trim();
            if (siteNumber != "")
            {
                return int.Parse(siteNumber);
            }
            else
            {
                return 0;
            }
        }

        //removes ending of url
        private string ReformatUrl(string url) 
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

        //Gets product type
        public string GetProductType(string tempLink)
        {
            tempLink = tempLink.ToLower();
            if (tempLink.Contains("articles-categories/videocards"))
            {
                return "GPU";
            }
            else if (tempLink.Contains("articles-categories/processors"))
            {
                return "CPU";
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
            else
            {
                Debug.WriteLine("couldnt determine product type - GetProductType, guru3d");
            }

            return "";
        }

        //parses review content and saves in review
        public override bool Parse(string siteData, string sQueueData)
        {
            string siteContentParsed = removeTagsFromReview(siteData);
            
            if (!currentSite.Contains("articles-summary")) // if not comment page of review
            {
                if (currentSite.Contains(",1.html")) // first page of review
                {
                    review = new Review(currentSite, sQueueData, true);
                    review.title = GetTitle(siteData);
                    review.productRating = GetRating(siteData);
                    review.maxRating = maxRating;
                    review.crawlDate = DateTime.Now;
                    review.reviewDate = GetReviewDate(siteData);
                    review.author = "Guru3d";
                }
                review.content += siteContentParsed;
            }
            else // if comment page of review
            {
                review.comments = GetReviewComments(siteData);
                
                //for testing purposes only
                if (this.GetType() == typeof(SiteGuru3d))
                {
                    MainWindow.guru3d++;
                }

                return true; //review is done
            }
            return false; // review is not done yet
        }

        // gets title of review
        private string GetTitle(string data)
        {
            string result = "";

            result =
                Regex.Match(data, "<meta itemprop=\"name\" property=\"og:title\" content=\".*?/>",
                    RegexOptions.Singleline).Value;

            result = result.Replace("<meta itemprop=\"name\" property=\"og:title\" content=\"", "");
            result = result.Replace("\" />", "");

            return result;
        }

        //Gets comments for review
        private List<ReviewComment> GetReviewComments(string siteData) //look into nested quotes
        {
            List<ReviewComment> commentResults = new List<ReviewComment>();
            Regex regexTags = new Regex("(<.*?>)+", RegexOptions.Singleline);
            Regex regexDateRemove = new Regex("(<strong>Posted on:(.*?)</strong>)", RegexOptions.Singleline);
            Regex regexQuote = new Regex("(<table(.*?)</table>)", RegexOptions.Singleline);
            Regex regexQuoteBlock = new Regex("(<div class=\"quoteblock(.*?)</div>)", RegexOptions.Singleline);
            string tempComment;

            string[] commentSplit = siteData.Split(new string[] {"<div class=\"comments\">"}, StringSplitOptions.None);

            for (int i = 0; i < commentSplit.Length; i++)
            {
                tempComment =
                    Regex.Match(commentSplit[i], "(<strong>Posted on:.*?<!-- Template: articles)",
                        RegexOptions.Singleline).Value;
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

        //Gets review date
        private DateTime GetReviewDate(string siteData)
        {
            DateTime date;
            string temp = Regex.Match(siteData, "<span itemprop=\"dtreviewed\">.*?</span>").Value;

            temp = temp.Replace("<span itemprop=\"dtreviewed\">", "");
            temp = temp.Replace("</span>", "");

            string[] tempDate = temp.Split('/');
            if (tempDate.Length > 1)
            {
                if (tempDate[2].Length > 3)
                {
                    tempDate[2] = tempDate[2].Substring(0, 4);
                }
                else
                {
                    Debug.WriteLine("Date format error");
                    return DateTime.Now; 
                }
                
                if ((tempDate[0])[0] == '0')
                {
                    tempDate[0] = (tempDate[0])[1].ToString();
                }

                date = new DateTime(int.Parse(tempDate[2]), int.Parse(tempDate[0]), int.Parse(tempDate[1]));
            }
            else
            {
                return DateTime.Now;
            }

            return date;
        }

        //Gets review rating
        private double GetRating(string siteData)
        {
            string temp = Regex.Match(siteData, "<meta itemprop=\"value\" content=\".*?\"/>").Value;

            if (temp != "")
            {
                return double.Parse(temp);
            }
            else
            {
                return -1;
            }
        }
    }
}