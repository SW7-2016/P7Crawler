using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Helpers;
using System.Text.RegularExpressions;
using ReviewCrawler.Products.Reviews;

namespace ReviewCrawler.Sites.Sub
{
    class SiteComputerShopper : ReviewSite
    {
        private const double maxRating = 5;

        public SiteComputerShopper()
        {
            domainUrl = "http://www.computershopper.com";
            searchQueue.Enqueue(new QueueElement("http://www.computershopper.com/components/graphics-card/reviews", "GPU")); //finished
            searchQueue.Enqueue(new QueueElement("http://www.computershopper.com/components/processor/reviews", "CPU")); //finished
            searchQueue.Enqueue(new QueueElement("http://www.computershopper.com/components/motherboard/reviews", "Motherboard")); //finished
            searchQueue.Enqueue(new QueueElement("http://www.computershopper.com/components/pc-case/reviews", "Chassis")); //finished
        }

        public override void Crawl(string siteData, string sQueueData)
        {
            string[] articlelinks;
            if (!currentSite.Contains("/components/reviews/"))
            {
                articlelinks = regexMatches(siteData, "<span class=\"title\"><a href=\"", "\">");

                for (int i = 0; i <= articlelinks.Length - 1; i++)
                {
                    searchQueue.Enqueue(new QueueElement(domainUrl + articlelinks[i], sQueueData));
                }
            }
            else
            {
                string tempNextLink = "";
                itemQueue.Enqueue(new QueueElement(currentSite, sQueueData));
                if (siteData.Contains("<div class=\"article-utility\">"))
                {
                    tempNextLink = regexMatch(siteData, "<div class=\"review-nav\">", "</ul>");
                    articlelinks = regexMatches(tempNextLink, "<a href=\"", "\">");
                    for (int i = 1; i <= articlelinks.Length - 1; i++)
                    {
                        itemQueue.Enqueue(new QueueElement(domainUrl + articlelinks[i], sQueueData));
                    }
                }

            }
        }

        public override bool Parse(string siteData, string sQueueData)
        {

            string siteContentParsed = removeTagsFromReview(siteData);
            siteContentParsed = siteContentParsed.Replace("TERMS OF USE", "");
            siteContentParsed = siteContentParsed.Replace("ComputerShopper may earn affiliate commissions from shopping links included on this page. To find out more, read our complete Terms of Service.", "");
            if (!currentSite.Contains("(page)"))
            {
                review = new Review(currentSite, sQueueData, true);
                review.title = regexMatch(siteData, "<title>", "Review - ComputerShopper.com</title>  ");
                review.productRating = GetReviewRating(siteData);
                review.maxRating = maxRating;
                review.crawlDate = DateTime.Now;
                review.reviewDate = GetReviewDate(siteData);
                review.author = "ComputerShopper";
            }
            review.content += siteContentParsed;

            if (itemQueue.Count == 0)
            {
                MainWindow.computershopper++;
                return true;
            }
            return false;
        }

        private double GetReviewRating(string data)
        {
            string[] tempRatingSplit;

            string tempRating = regexMatch(data, "<b></b><em>Rated <span itemprop=\"ratingValue\">",
                "</span>/<span itemprop=\"bestRating\"");
            if (tempRating != "")
            {
                if (tempRating.Contains("."))
                {
                    tempRatingSplit = tempRating.Split('.');
                    return double.Parse(tempRatingSplit[0]) + (double.Parse(tempRatingSplit[1]) / 10);
                }
                else
                {
                    return double.Parse(tempRating);
                }
            }
            else
            {
                return -1;
            }
            
        }

        private DateTime GetReviewDate(string data)
        {
            string tempString = regexMatch(data, "class=\"dtreviewed\">", "</span>").Replace(",", "");
             
            string[] date = tempString.Split(' ');

            if (date[0].ToLower().Contains("january"))
            {
                date[0] = "1";
            }
            else if (date[0].ToLower().Contains("february"))
            {
                date[0] = "2";
            }
            else if (date[0].ToLower().Contains("march"))
            {
                date[0] = "3";
            }
            else if (date[0].ToLower().Contains("april"))
            {
                date[0] = "4";
            }
            else if (date[0].ToLower().Contains("may"))
            {
                date[0] = "5";
            }
            else if (date[0].ToLower().Contains("june"))
            {
                date[0] = "6";
            }
            else if (date[0].ToLower().Contains("july"))
            {
                date[0] = "7";
            }
            else if (date[0].ToLower().Contains("august"))
            {
                date[0] = "8";
            }
            else if (date[0].ToLower().Contains("september"))
            {
                date[0] = "9";
            }
            else if (date[0].ToLower().Contains("october"))
            {
                date[0] = "10";
            }
            else if (date[0].ToLower().Contains("november"))
            {
                date[0] = "11";
            }
            else if (date[0].ToLower().Contains("december"))
            {
                date[0] = "12";
            }

            date[1] = date[1].Replace(",", "");

            return new DateTime(int.Parse(date[2]), int.Parse(date[0]), int.Parse(date[1]));
        }

        public override string GetProductType(string tempLink)
        {
            return "";
        }
    }
}