using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites
{
    abstract class Host : HostInterface
    {
        public DateTime timeStamp = DateTime.Now;
        public Queue<string> reviewQueue = new Queue<string>();
        public Queue<string> searchQueue = new Queue<string>();


        public abstract void Parse();
        public abstract void CrawlPage();

        public bool Crawl()
        {

            //stuff

            CrawlPage(); //Handles the site specific crawling, is overwritten in subclasses

            //Checks if there is more content to crawl on this host
            if (reviewQueue.Count < 1 && searchQueue.Count < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

        public DateTime GetLastAccessTime()
        {
            return timeStamp;
        }
    }
}
