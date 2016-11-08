using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites
{
    abstract class ReviewSite : Site
    {
        public override abstract void Parse(string siteData);
        public override abstract void CrawlPage(string siteData);
        public override abstract string GetSiteKey(string url);
        public override abstract void CrawlReviewPages(string siteData);

        public string removeTagsFromReview(string siteData)
        {

            string tempString = "";

            foreach (Match item in Regex.Matches(siteData, "((<p>|<p style|<p align=\"left\">).*?(<\\/p>))+", RegexOptions.IgnoreCase))
            {
                tempString += item + "\n";
            }

            Regex newlineAdd = new Regex("<br />", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex regexHtml = new Regex("(<.*?>)+", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex apostropheRemover = new Regex("\\&rsquo\\;", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Regex garbageRemover = new Regex("\\&nbsp\\;", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            tempString = newlineAdd.Replace(tempString, "\n");
            tempString = regexHtml.Replace(tempString, "");
            tempString = apostropheRemover.Replace(tempString, "");
            tempString = garbageRemover.Replace(tempString, " ");

            tempString += "\n";

            return tempString;
        }

    }
}
