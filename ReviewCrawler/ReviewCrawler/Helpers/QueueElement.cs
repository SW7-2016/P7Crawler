using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Helpers
{
    class QueueElement
    {
        public string url;
        public string data;

        public QueueElement(string Url, string Data)
        {
            url = Url;
            data = Data;
        }
    }
}
