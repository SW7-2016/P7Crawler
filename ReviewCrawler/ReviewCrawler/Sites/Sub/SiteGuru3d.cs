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

        public SiteGuru3d()
        {
            domainUrl = "http://www.guru3d.com/";
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/videocards.html");
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/processors.html");
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/soundcards-and-speakers.html");
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/mainboards.html");
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/memory-(ddr2%7Cddr3)-and-storage-(hdd%7Cssd).html");
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/pc-cases-and-modding.html");
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/psu-power-supply-units.html");
            searchQueue.Enqueue("http://www.guru3d.com/articles-categories/cooling.html");
        }

        public override void CrawlPage(string siteData)
        {
            string tempLink = "";
            List<string> tempReviewLinks;

            tempLink = GetSearchLinks(siteData, "pagelinkselected", "pagelink", false); //Returns domainUrl if no link is found
            if (tempLink != domainUrl)
            {
                searchQueue.Enqueue(tempLink);
            }
     
            tempReviewLinks = GetReviewLinks(siteData, "<br />", "<a href=\"articles-pages", "<div class=\"content\">", true);
            foreach (var item in tempReviewLinks)
            {
                
                    reviewQueue.Enqueue(item);
                
                
                if (!Crawler.reviews.ContainsKey(GetSiteKey(item)))
                {
                    Crawler.reviews.Add(GetSiteKey(item), new Review(item, GetProductType(currentSite)));
                }
            }

        }

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
                    //GetSiteKey(), just so happens to give the wanted output, even though it is not used as a key here


                    reviewQueue.Enqueue(tempLines[0]);
                }
            }

        }

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
            else if (tempLink.Contains("articles-categories/memory-(ddr2%7Cddr3)-and-storage-(hdd%7Cssd)"))
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

            if (Crawler.reviews.ContainsKey(GetSiteKey(currentSite)))
            {
                Crawler.reviews[GetSiteKey(currentSite)].content += siteContentParsed;
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
