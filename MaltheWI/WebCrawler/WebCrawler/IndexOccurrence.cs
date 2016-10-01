using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class IndexOccurrence
    {
        public string term;
        public double idf;
        public Dictionary<int, double> pages = new Dictionary<int, double>();

        public IndexOccurrence(string Term)
        {
            term = Term;
        }
    }
}
