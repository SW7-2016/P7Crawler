using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReviewCrawler.Helpers;
using ReviewCrawler.Products.Reviews;

namespace ReviewCrawler.Sites.Sub
{
    class SiteTechPowerUp : ReviewSite
    {
        private const double maxRating = 10;

        public SiteTechPowerUp()
        {
            domainUrl = "https://www.techpowerup.com";

            //searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=Power+Supplies&manufacturer=&pp=10000&order=date", "")); //finished
            //searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=Motherboards&manufacturer=&pp=10000&order=date", ""));
            searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=Processors&manufacturer=&pp=10000&order=date", "")); 
            //searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=SSD&manufacturer=&pp=10000&order=date", "")); //Finished
            //searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=External+HDDs&manufacturer=&pp=10000&order=date", "")); //finished
            //searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=Memory&manufacturer=&pp=10000&order=date", "")); //finished
            //searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=Cases&manufacturer=&pp=10000&order=date", ""));  //finished
            searchQueue.Enqueue(new QueueElement("https://www.techpowerup.com/reviews/?category=Graphics+Cards&manufacturer=&pp=10000&order=date", ""));
        }

        public override void CrawlPage(string siteData, string sQueueData)
        {
            string tempLink = "";
            string link = "";
            int totalPages = 0;
            string score = "";

            if (currentSite.Contains("?category"))
            {
                string[] dataSplit = siteData.Split(new string[] {"<div class=\"title\">"}, StringSplitOptions.None);

                for (int i = 1; i <= dataSplit.Length - 1; i++)
                {
                    if (dataSplit[i].Contains("<div class=\"score\">"))
                    {
                        link = regexMatch(dataSplit[i], "<a  href=\"", "\">");
                        score = regexMatch(dataSplit[i], "<div class=\"score\"><span>Score:</span>", "</div>");
                        searchQueue.Enqueue(new QueueElement(domainUrl + link, score));
                    }
                }
            }
            else
            {
                string tempTotPages = regexMatch(siteData, "<select id=\"pagesel\">", "</select>");
                string[] tempPagesSplit = tempTotPages.Split(new string[] {"</option>"}, StringSplitOptions.None);
                if (tempPagesSplit.Length > 2)
                {
                    totalPages = int.Parse(regexMatch(tempPagesSplit[tempPagesSplit.Length - 3], "\" >", "-"));
                    for (int i = 1; i <= totalPages; i++)
                    {
                        itemQueue.Enqueue(new QueueElement(currentSite + i + ".html", sQueueData));
                    }
                }
            }
        }

        public override bool Parse(string siteData, string sQueueData)
        {
            string siteContentParsed = regexMatch(siteData, "<div class=\"text p\" >", "<footer class=\"clearfix\">");
            //Regex tableRemover = new Regex("<div class=\"table-wrapper\">.*?</table>", RegexOptions.Singleline);
            Regex divRemover = new Regex("<table class=.*?/table>", RegexOptions.Singleline);
            siteContentParsed = TagRemoval(divRemover.Replace(siteContentParsed, ""));
            

            if (currentSite.Contains("/1.html"))
            {
                review = new Review(currentSite, GetProductType(siteData), true);
                review.title = regexMatch(siteData, "<title>", "Review | techPowerUp</title>");
                review.productRating = GetReviewRating(sQueueData);
                review.maxRating = maxRating;
                review.crawlDate = DateTime.Now;
                review.reviewDate = GetReviewDate(siteData);
                review.author = "TechPowerUp";
            }
            review.content += siteContentParsed;

            if (regexMatch(siteData, "<a class=\"button nextpage-bottom\"", "User comments\\)</small>") != "")
            {
                MainWindow.techpowerup++;
                return true;
            }
            return false;
        }


        private DateTime GetReviewDate(string data)
        {
            string[] tempDate = regexMatch(data, "<span>on <time datetime=\"", "T").Split('-');

            return new DateTime(int.Parse(tempDate[0]), int.Parse(tempDate[1]), int.Parse(tempDate[2]));
        }

        public override string GetProductType(string data)
        {
            string temp = regexMatch(data, "<span>in <a href=\"/reviews/", "</a>.</span>").ToLower();

            if (temp.Contains("graphics cards"))
            {
                return "GPU";
            }
            else if (temp.Contains("processors"))
            {
                return "CPU";
            }
            else if (temp.Contains("motherboards"))
            {
                return "Motherboard";
            }
            else if (temp.Contains("ssd") || temp.Contains("external hdd"))
            {
                return "HDD";
            }
            else if (temp.Contains("cases"))
            {
                return "Chassis";
            }
            else if (temp.Contains("power supplies"))
            {
                return "PSU";
            }
            else if (temp.Contains("memory"))
            {
                return "RAM";
            }
            else
            {
                Debug.WriteLine("couldnt determine product type - GetProductType, guru3d");
            }


            return "unknown";
        }

        public double GetReviewRating(string scoreStr)
        {
            double result = 0;
            string[] tempRatingSplit = scoreStr.Split('.');
            result = double.Parse(tempRatingSplit[0]) + (double.Parse(tempRatingSplit[1])/10);

            return result;
        }
    }
}