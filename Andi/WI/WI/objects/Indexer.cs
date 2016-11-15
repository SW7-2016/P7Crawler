using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI.objects
{
    public class Indexer
    {
        public Dictionary<string, IndexToken> index = new Dictionary<string, IndexToken>();
        
        public void CreateIndex(int siteId, List<string> tokens)
        {

            for (int i = 1; i < tokens.Count(); i++)
            {
                if (!index.ContainsKey(tokens[i]))
                {
                    index.Add(tokens[i], new IndexToken(tokens[i]));
                    index[tokens[i]].pages.Add(siteId, 0);
                    index[tokens[i]].pages[siteId]++;
                    MainWindow.IndexedTokens++;
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

        public Dictionary<int, int> FindToken(string token)
        {
            if (index.ContainsKey(token + "\r"))
            {
                return index[token + "\r"].pages;
            }
            else
            {
                return null;
            }
        }
    }
}
