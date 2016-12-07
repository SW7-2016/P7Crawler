using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using ReviewCrawler.Helpers;
using ReviewCrawler.Products.Reviews;

namespace ReviewCrawler.Sites.Sub
{
    class SiteAmazon : ReviewSite
    {
        private double maxRating = 5;

        public SiteAmazon()
        {
            domainUrl = "https://www.amazon.com";
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_284822_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A284822&ie=UTF8&qid=1479384287&lo=computers","state1,GPU"));
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_229189_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A229189&ie=UTF8&qid=1479457498&lo=computers", "state1,CPU"));
            //searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_1048424_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A1048424&ie=UTF8&qid=1479457799&lo=computers", "state1,Motherboard"));
            //searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_2248325011_il_ti_electronics?rh=n%3A172282%2Cn%3A%2113900871%2Cn%3A%212334091011%2Cn%3A%212334122011%2Cn%3A2248325011&ie=UTF8&qid=1479458095&lo=electronics", "state1,HDD"));
            //searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=sr_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A172500&ie=UTF8&qid=1479458749&lo=computers", "state1,RAM"));
            //searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_572238_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A572238&ie=UTF8&qid=1479458873&lo=computers", "state1,Chassis"));
            //searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_1161760_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A1161760&ie=UTF8&qid=1479459093&lo=computers", "state1,PSU"));
        }

        public override void Crawl(string siteData, string queueData)
        {
            string nextPageLink = "";
            string[] articleLinks;
            string tempProductType = (queueData.Split(','))[1];
            

            if (queueData.Contains("state1"))
            {
                //Gets match, without identifiers.
                
                nextPageLink = regexMatch(siteData, "class=\"pagnNext\"", "<span id=\"pagnNextString\">Next Page</span>");
                nextPageLink = regexMatch(nextPageLink, "href=\"", "\">");

                if (nextPageLink != "")
                {
                    if (!nextPageLink.Contains(domainUrl))
                    {
                        searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink.Replace("&amp;", "&"), "state1," + tempProductType));
                    }
                    else
                    {
                        searchQueue.Enqueue(new QueueElement(nextPageLink.Replace("&amp;", "&"), "state1," + tempProductType));
                    }
                    
                }
                //Gets matches, without identifiers.
                if (siteData.Contains("<div id=\"atfResults\""))
                {
                    string[] data = siteData.Split(new string[] { "<div id=\"atfResults\"" }, StringSplitOptions.None);
                    articleLinks = regexMatches(data[1], "<a class=\"a-link-normal a-text-normal\" href=\"", "\"><img src=\"");
                }
                else
                {
                    articleLinks = regexMatches(siteData, "<a class=\"a-link-normal a-text-normal\" href=\"", "\"><img src=\"");
                }
                
                //"><a class="a-link-normal a-text-normal" href=".*?"><img src="
                foreach (string link in articleLinks)
                {
                    searchQueue.Enqueue(new QueueElement(link.Replace("&amp;", "&"), "state2," + tempProductType));
                }
            }
            else if (queueData.Contains("state2"))
            {
                nextPageLink = regexMatch(siteData, "<a class=\"a-link-emphasis a-nowrap\" href=\"", "\">See all");
                if (nextPageLink != "")
                {
                    searchQueue.Enqueue(new QueueElement(nextPageLink, "state3," + tempProductType));
                }
                
            }
            else if (queueData.Contains("state3"))
            {

                nextPageLink = regexMatch(siteData, "<link rel=\"next\" href=\"", "\" />");
                if (nextPageLink != "" && nextPageLink.Contains(domainUrl))
                {
                    searchQueue.Enqueue(new QueueElement(nextPageLink, "state3," + tempProductType));
                }
                else if (!nextPageLink.Contains(domainUrl))
                {
                    searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink, "state3," + tempProductType));
                }

                

                articleLinks = regexMatches(siteData, "class=\"a-size-base a-link-normal review-title a-color-base a-text-bold\" href=\"", "\">");
                foreach (var review in articleLinks)
                {
                    itemQueue.Enqueue(new QueueElement(domainUrl + review, "state4," + tempProductType));
                }

            }
        }

        public override bool Parse(string siteData, string queueData)
        {
            string siteContentParsed = regexMatch(siteData, "<div class=\"reviewText\">", "</div>");
            siteContentParsed = TagRemoval(siteContentParsed);

            review = new Review(currentSite, (queueData.Split(','))[1], false);
            string tempTitle = regexMatch(siteData, "<div class=\"crDescription\">", "<span class=\"crAvgStars\"");
            review.title = TagRemoval(tempTitle).Trim();
            string tempRating = regexMatch(siteData, "title=\"", "out of 5 stars");
            review.productRating = double.Parse(tempRating[0] + "," + tempRating[2]);
            review.maxRating = maxRating;
            review.crawlDate = DateTime.Now;
            review.reviewDate = GetReviewDate(siteData);
            review.author = GetAuthor(siteData);
            GetReviewSentimentCount(siteData);

            review.verifiedPurchase = IsVerified(siteData);

            review.content += siteContentParsed;

            //for testing purposes only
            if (this.GetType() == typeof(SiteAmazon))
            {
                MainWindow.amazon++;
            }

            return true;
        }
        
        private void GetReviewSentimentCount(string data)
        {
            string temp = regexMatch(data, "<div style=\"margin-bottom:0.5em;\">", "people found the following review helpful");
            if (temp.Contains("of"))
            {
                string[] tempSplit = temp.Split(new string[] { "of" }, StringSplitOptions.None);

                tempSplit[0] = CheckForAndremoveComma(tempSplit[0]);
                tempSplit[1] = CheckForAndremoveComma(tempSplit[1]);
                review.positiveReception = int.Parse(tempSplit[0].Trim());
                review.negativeReception = (int.Parse(tempSplit[1].Trim()) - int.Parse(tempSplit[0].Trim()));
            }
            else
            {
                review.negativeReception = 0;
                review.positiveReception = 0;
            }
        }

        private string CheckForAndremoveComma(string str)
        {
            if (str.Contains(","))
            {
                return str.Replace(",", "");
            }
            else
            {
                return str;
            }
        }

        private string GetAuthor(string data)
        {
            string result = regexMatch(data, "<span style = \"font-weight: bold;\">", "</span>");
            if (result != "")
            {
                return result;
            }
            else
            {
                return "Unknown";
            }
        }

        private DateTime GetReviewDate(string data)
        {
            string[] date = (regexMatch(data, ", <nobr>", "</nobr>")).Split(' ');

            if (date[0].ToLower().Contains("january"))
            {
                date[0] = "1";
            } else if (date[0].ToLower().Contains("february"))
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

        public bool IsVerified(string data)
        {
            if (data.Contains("\">Verified Purchase</b>"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string GetProductType(string tempLink)
        {
            return "";
        }

        
    }
}