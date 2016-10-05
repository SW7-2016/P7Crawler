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

            if (!isReview)
            {
                GetSearchLinks(siteData);
                GetReviewLinks(siteData);
            }
            else if (isReview)
            {
                Parse(siteData);
            }

        }

        public List<string> GetReviewLinks(string siteData)
        {
            List<string> reviewLinks = new List<string>();




            return reviewLinks;
        }

        public List<string> GetSearchLinks(string siteData)
        {
            List<string> searchLinks = new List<string>();

            string[] lines = siteData.Split('\n');


            return searchLinks;
        }

        public override void Parse(string siteData)
        {

        }

    }
}
