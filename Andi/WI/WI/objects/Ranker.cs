using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI.objects
{
    public class Ranker
    {
        public Dictionary<string, RankedToken> index = new Dictionary<string, RankedToken>();

        public Ranker(Dictionary<string, IndexToken> Index)
        {
            int documentCount = Index.Count();
            foreach (KeyValuePair<string, IndexToken> token in Index)
            {
                index.Add(token.Key, new RankedToken(token.Value.token, token.Value.pages, documentCount));
            }
        }
    }
}
