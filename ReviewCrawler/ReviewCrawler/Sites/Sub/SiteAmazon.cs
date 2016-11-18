﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ReviewCrawler.Helpers;
using ReviewCrawler.Products.Reviews;

namespace ReviewCrawler.Sites.Sub
{
    class SiteAmazon : ReviewSite
    {
        private double maxRating = 5;
        List<Review> reviewList = new List<Review>();

        public SiteAmazon()
        {
            domainUrl = "https://www.amazon.com";
            //searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_284822_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A284822&ie=UTF8&qid=1479384287&lo=computers","state1,GPU"));
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_229189_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A229189&ie=UTF8&qid=1479457498&lo=computers", "state1,CPU"));
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_1048424_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A1048424&ie=UTF8&qid=1479457799&lo=computers", "state1,Motherboard"));
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_2248325011_il_ti_electronics?rh=n%3A172282%2Cn%3A%2113900871%2Cn%3A%212334091011%2Cn%3A%212334122011%2Cn%3A2248325011&ie=UTF8&qid=1479458095&lo=electronics", "state1,HDD"));
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=sr_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A172500&ie=UTF8&qid=1479458749&lo=computers", "state1,RAM"));
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_572238_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A572238&ie=UTF8&qid=1479458873&lo=computers", "state1,Chassis"));
            searchQueue.Enqueue(new QueueElement("https://www.amazon.com/s/ref=lp_1161760_il_ti_computers?rh=n%3A172282%2Cn%3A%21493964%2Cn%3A541966%2Cn%3A193870011%2Cn%3A1161760&ie=UTF8&qid=1479459093&lo=computers", "state1,PSU"));
        }

        public override void CrawlPage(string siteData, string queueData)
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
                nextPageLink = regexMatch(siteData, "<a class=\"a-link-emphasis a-nowrap\" href=\"", "\">See all verified purchase reviews</a>");
                if (nextPageLink != "")
                {
                    searchQueue.Enqueue(new QueueElement(nextPageLink, "state3," + tempProductType));
                    itemQueue.Enqueue(new QueueElement(nextPageLink, "state3," + tempProductType)); //input is messed up for nextpagelink
                }
                
            }
            else if (queueData.Contains("state3"))
            {
                if (nextPageLink != "")
                {
                    nextPageLink = regexMatch(siteData, "<link rel=\"next\" href=\"", "\" />");
                    searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink, "state3," + tempProductType));
                }
            }
        }

        public override bool Parse(string siteData, string queueData)
        {
            reviewList.Clear();
            string[] reviewSplit = siteData.Split(new string[] {"<i data-hook=\"review-star-rating\""},
                StringSplitOptions.None);

            string tempTitle = regexMatch(reviewSplit[0], "<title>Amazon.com: Customer Reviews:", "</title>");

            for (int i = 1; i < reviewSplit.Length - 1; i++)
            {
                string siteContentParsed = regexMatch(reviewSplit[i], "review-text\">", "</span></div>"); //removeTagsFromReview(reviewSplit[i]);
                siteContentParsed = TagRemoval(siteContentParsed);

                review = new Review(currentSite + i, (queueData.Split(','))[1], false);
                review.title = tempTitle;
                string tempRating = regexMatch(reviewSplit[i], "<span class=\"a-icon-alt\">", "out of 5 stars</span>");
                review.productRating = double.Parse(tempRating[0] + "," + tempRating[2]);
                review.maxRating = maxRating;
                review.crawlDate = DateTime.Now;
                review.reviewDate = GetReviewDate(reviewSplit[i]);
                review.verifiedPurchase = true;

                review.content += siteContentParsed;

                reviewList.Add(review);
            }
            return true;
        }

        private DateTime GetReviewDate(string data)
        {
            string[] date = (regexMatch(data, "class=\"a-size-base a-color-secondary review-date\">on ", "</span>")).Split(' ');

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

        public override void AddItemToDatabase(MySqlConnection connection)
        {
            foreach (var rw in reviewList)
            {
                rw.connection = connection;
                rw.AddReviewToDB();
            }
        }

        public override string GetProductType(string tempLink)
        {
            return "";
        }

        public override string GetSiteKey(string url)
        {
            /*for (int i = url.Length; i > 0; i--)
            {
                if (url[i] == ',')
                {
                    url = url.Remove(i, url.Length - i);

                }
            }
            */
            return url;
        }
    }
}