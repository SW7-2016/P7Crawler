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

            if (!isReview)
            {
                tempLink = GetSearchLinks(siteData, "pagelinkselected", "pagelink", false); //Returns domainUrl if no link is found
                if (tempLink != domainUrl)
                {
                    searchQueue.Enqueue(tempLink);
                }
                
                GetReviewLinks(siteData, "<br />", "<a href=\"articles-pages", "<div class=\"content\">", true);
            }
            else if (isReview)
            {
                Parse(siteData);
            }

        }
       

        

        public override void Parse(string siteData)
        {

        }

    }
}
