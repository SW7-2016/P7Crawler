using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ReviewCrawler.Products.Reviews;

namespace ReviewCrawler.Sites
{
    abstract class ReviewSite : Site
    {
        public Review review;

        public override abstract bool Parse(string siteData, string sQueueData);
        public override abstract void Crawl(string siteData, string sQueueData);

        //Removes tags and other html parts from a string + only keeps what is within <p> <\p>
        public string removeTagsFromReview(string siteData)
        {
            string tempString = "";

            foreach (
                Match item in
                Regex.Matches(siteData, "((<p>|<p style|<p align=\"left\">).*?(<\\/p>))+", RegexOptions.IgnoreCase))
            {
                tempString += item + "\n";
            }

            tempString = TagRemoval(tempString);

            return tempString;
        }
        //Removes tags and other html parts from a string 
        public string TagRemoval(string tempString)
        {
            Regex newlineAdd = new Regex("<br />", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex regexHtml = new Regex("(<.*?>)+", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex apostropheRemover = new Regex("\\&rsquo\\;", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex garbageRemover = new Regex("\\&nbsp\\;", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            tempString = newlineAdd.Replace(tempString, " ");
            tempString = regexHtml.Replace(tempString, "");
            tempString = apostropheRemover.Replace(tempString, "");
            tempString = garbageRemover.Replace(tempString, " ");

            tempString += "\n";

            return tempString;
        }

        protected int GetReviewDateParseMonth(string month)
        {
            if (month.ToLower().Contains("january"))
            {
                month = "1";
            }
            else if (month.ToLower().Contains("february"))
            {
                month = "2";
            }
            else if (month.ToLower().Contains("march"))
            {
                month = "3";
            }
            else if (month.ToLower().Contains("april"))
            {
                month = "4";
            }
            else if (month.ToLower().Contains("may"))
            {
                month = "5";
            }
            else if (month.ToLower().Contains("june"))
            {
                month = "6";
            }
            else if (month.ToLower().Contains("july"))
            {
                month = "7";
            }
            else if (month.ToLower().Contains("august"))
            {
                month = "8";
            }
            else if (month.ToLower().Contains("september"))
            {
                month = "9";
            }
            else if (month.ToLower().Contains("october"))
            {
                month = "10";
            }
            else if (month.ToLower().Contains("november"))
            {
                month = "11";
            }
            else if (month.ToLower().Contains("december"))
            {
                month = "12";
            }
            else
            {
                month = "1";
            }

            return int.Parse(month);
        }

        public override void AddItemToDatabase(MySqlConnection connection)
        {
            review.AddReviewToDB(connection);
        }
    }
}