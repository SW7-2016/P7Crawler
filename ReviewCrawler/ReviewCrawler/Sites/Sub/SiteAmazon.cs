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
            
            if (queueData.Contains("state1"))  //ProductGroup page
            {
                nextPageLink = GetNextProductGroupPage(siteData);

                if (nextPageLink != "")
                {
                    //In some instances nextpagelink will already contain the domainURL while in others it will not
                    if (!nextPageLink.Contains(domainUrl))  
                    {
                        searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink.Replace("&amp;", "&"), "state1," + tempProductType));
                    }
                    else
                    {
                        searchQueue.Enqueue(new QueueElement(nextPageLink.Replace("&amp;", "&"), "state1," + tempProductType));
                    }
                }

                articleLinks = GetProductPageLinks(siteData); //gets the url for all products on current productGroupPage

                foreach (string link in articleLinks)
                {
                    searchQueue.Enqueue(new QueueElement(link.Replace("&amp;", "&"), "state2," + tempProductType));
                }
            }
            else if (queueData.Contains("state2")) //if on a productPage
            {
                //Gets url to reviewListPage for product
                nextPageLink = regexMatch(siteData, "<a class=\"a-link-emphasis a-nowrap\" href=\"", "\">See all");
                if (nextPageLink != "")
                {
                    searchQueue.Enqueue(new QueueElement(nextPageLink, "state3," + tempProductType));
                }
            }
            else if (queueData.Contains("state3")) //if on reviewListPage
            {
                //gets next page reviewListPage for same product
                nextPageLink = regexMatch(siteData, "<link rel=\"next\" href=\"", "\" />");
                if (nextPageLink != "" && nextPageLink.Contains(domainUrl))
                {
                    searchQueue.Enqueue(new QueueElement(nextPageLink, "state3," + tempProductType));
                }
                else if (!nextPageLink.Contains(domainUrl))
                {
                    searchQueue.Enqueue(new QueueElement(domainUrl + nextPageLink, "state3," + tempProductType));
                }
                //Gets url for individual reviews on reviewListPage
                articleLinks = regexMatches(siteData, "class=\"a-size-base a-link-normal review-title a-color-base a-text-bold\" href=\"", "\">");
                foreach (var review in articleLinks)
                {
                    itemQueue.Enqueue(new QueueElement(domainUrl + review, "state4," + tempProductType));
                }
            }
        }

        //Gets nextpage link for a productGroupPage
        private string GetNextProductGroupPage(string siteData)
        {
            string nextPageLink = regexMatch(siteData, "class=\"pagnNext\"", "<span id=\"pagnNextString\">Next Page</span>");
            return regexMatch(nextPageLink, "href=\"", "\">");
        }

        //Gets links to each individual product, on a productGroupPage
        private string[] GetProductPageLinks(string siteData)
        {
            if (siteData.Contains("<div id=\"atfResults\"")) //if it is not the first productGroupPage
            {
                string[] data = siteData.Split(new string[] { "<div id=\"atfResults\"" }, StringSplitOptions.None);
                return regexMatches(data[1], "<a class=\"a-link-normal a-text-normal\" href=\"", "\"><img src=\"");
            }
            else // if on first productGroupPage
            {
                return regexMatches(siteData, "<a class=\"a-link-normal a-text-normal\" href=\"", "\"><img src=\"");
            }
        }

        //Parses the data from a review and saves it in review
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

            return true; //review is done
        }
        
        //gets and updates review helpfull/unhelpfull count
        private void GetReviewSentimentCount(string data)
        {
            string temp = regexMatch(data, "<div style=\"margin-bottom:0.5em;\">", "people found the following review helpful");
            if (temp.Contains("of"))
            {
                string[] tempSplit = temp.Split(new string[] { "of" }, StringSplitOptions.None);

                tempSplit[0] = tempSplit[0].Replace(",", "");
                tempSplit[1] = tempSplit[1].Replace(",", "");
                review.positiveReception = int.Parse(tempSplit[0].Trim());
                review.negativeReception = (int.Parse(tempSplit[1].Trim()) - int.Parse(tempSplit[0].Trim()));
            }
            else
            {
                review.negativeReception = 0;
                review.positiveReception = 0;
            }
        }

        //Gets the author from review
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

        //Gets the time of review creation
        private DateTime GetReviewDate(string data)
        {
            string[] date = (regexMatch(data, ", <nobr>", "</nobr>")).Split(' ');
            date[1] = date[1].Replace(",", "");
            
            return new DateTime(int.Parse(date[2]), GetReviewDateParseMonth(date[0]), int.Parse(date[1]));
        }

        //does the review have a verified purchase
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
    }
}