using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class SResult
    {
        public int id;
        public double score;

        public SResult(int Id, double Score)
        {
            id = Id;
            score = Score;
        }
    }
}
