using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI.objects
{
    class Indexer
    {
        public Dictionary<string, IndexToken> index = new Dictionary<string, IndexToken>();


        public void CreateIndex(int siteId, List<string> tokens)
        {

            for (int i = 0; i < tokens.Count(); i++)
            {
                if (!index.ContainsKey(tokens[i]))
                {
                    index.Add(tokens[i], new IndexToken(tokens[i]));
                    index[tokens[i]].pages.Add(siteId, 0);
                    index[tokens[i]].pages[siteId]++;
                }
                else
                {
                    if (!index[tokens[i]].pages.ContainsKey(siteId))
                    {
                        index[tokens[i]].pages.Add(siteId, 0);
                    }
                    index[tokens[i]].pages[siteId]++;
                }
            }
            foreach (var item in index)
            {
                item.Value.pages.OrderByDescending(x => x.Value).ToList();
            }
        }
    }
}
