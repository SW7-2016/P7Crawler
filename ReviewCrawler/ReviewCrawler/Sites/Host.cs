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

    }
}
