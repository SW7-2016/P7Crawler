using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class Site 
    {
        public string url;
        public int id;
        public Host parent;
        public string content;

        public Site(string Url, string Content)
        {
            url = Url;
            content = Content;
            id = Crawler.siteId;
            Crawler.siteId++;
            string dom = ParseUrl();

            bool found = false;
            for (int i = 0; i < Crawler.backList.Count(); i++)
            {
                if (Crawler.backList[i].domain == dom)
                {
                    parent = Crawler.backList[i];
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                Host newHost = new Host(dom);
                Crawler.backList.Add(newHost);
                parent = newHost;
            }
        }

        public string ParseUrl()
        {
            string link = url;
            int slashCount = 0;
            for (int i = 0; i < link.Count(); i++)
            {
                if (link[i] == '/')
                {
                    if (slashCount == 2)
                    {
                        link = link.Remove(i, link.Count() - i);
                        break;
                    }

                    slashCount++;
                }
                if (i == link.Count() - 1)
                {
                    link = link.Remove(i, link.Count() - i - 1);
                    break;
                }

            }

            return link;
        }
    }
}
