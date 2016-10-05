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
                tempLink = GetSearchLinks(siteData);
                if (tempLink != domainUrl)
                {
                    searchQueue.Enqueue(tempLink);
                }
                
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

        public string GetSearchLinks(string siteData)
        {
            string newSearchLink = "";
            string tempString = "";
            bool linkFound = false;
            bool copyLink = false;
            siteData = siteData.ToLower();

            string[] lines = siteData.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("pagelinkselected") && lines[i+1].Contains("pagelink"))
                {
                    for (int j = 0; j < lines[i+1].Length; j++)
                    {
                        if (copyLink == true)
                        {
                            if ((lines[i + 1])[j] == '"')
                            {
                                break;
                            }
                            tempString += (lines[i + 1])[j];
                        }

                        if ((lines[i+1])[j] == 'h'
                            && (lines[i + 1])[j+1] == 'r'
                            && (lines[i + 1])[j+2] == 'e'
                            && (lines[i + 1])[j+3] == 'f')
                        {
                            linkFound = true;
                        }
                        if (linkFound == true && (lines[i + 1])[j] == '"')
                        {
                            copyLink = true;
                        }
                        
                    }
                }
                if (copyLink == true)
                {
                    break;
                }
            }

            newSearchLink = (domainUrl + tempString);

            return newSearchLink;
        }

        public override void Parse(string siteData)
        {

        }

    }
}
