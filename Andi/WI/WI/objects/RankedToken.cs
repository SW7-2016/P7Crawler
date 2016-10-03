using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI.objects
{
    public class RankedToken
    {
        public string token;
        public double idf;
        public Dictionary<int, int> pageFreq = new Dictionary<int, int>();
        public Dictionary<int, double> TfIdf = new Dictionary<int, double>();

        public RankedToken(string Token, Dictionary<int, int> PageFreq, int NoOfPages)
        {
            token = Token;
            pageFreq = PageFreq;
            idf = Math.Log10(NoOfPages / pageFreq.Count());

            foreach (KeyValuePair<int, int> page in pageFreq)
            {
                TfIdf.Add(page.Key, (1 + Math.Log10((double)page.Value)) * idf);
            }
        }


    }
}
