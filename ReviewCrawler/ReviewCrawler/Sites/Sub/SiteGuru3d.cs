using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void CrawlPage(string currentSite, bool isReview)
        {
            string siteData = GetSiteData(currentSite);
            string tempLink = "";
            List<string> tempReviewLinks;
            string tempProductType;

            if (!isReview)
            {
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
            }
            else if (isReview)
            {
                Parse(siteData);
            }

        }

        public string GetProductType(string tempLink)
        {
            string temp = "";

            if (tempLink.Contains("articles-categories/videocards"))
            {
                temp = "GPU";
            }
            else if (tempLink.Contains("articles-categories/processors"))
            {
                temp = "CPU";
            }
            else if (tempLink.Contains("articles-categories/soundcards-and-speakers"))
            {
                temp = "SoundCard";
            }
            else if (tempLink.Contains("articles-categories/mainboards"))
            {
                temp = "Motherboard";
            }
            else if (tempLink.Contains("articles-categories/memory-(ddr2%7Cddr3)-and-storage-(hdd%7Cssd)"))
            {
                temp = "RAM/HDD";
            }
            else if (tempLink.Contains("articles-categories/pc-cases-and-modding"))
            {
                temp = "Chassis";
            }
            else if (tempLink.Contains("articles-categories/psu-power-supply-units"))
            {
                temp = "PSU";
            }
            else if (tempLink.Contains("articles-categories/cooling"))
            {
                temp = "Cooling";
            }

            return temp;
        }
       

        

        public override void Parse(string siteData)
        {

        }

    }
}
