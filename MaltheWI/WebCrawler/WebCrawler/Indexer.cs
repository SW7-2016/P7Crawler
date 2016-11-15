using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using System.Diagnostics;

namespace WebCrawler
{
    class Indexer
    {
        public List<string> tokens = new List<string>();
        public Dictionary<string, IndexOccurrence> index = new Dictionary<string, IndexOccurrence>();
        public Dictionary<string, IndexOccurrence> weightedIndex = new Dictionary<string, IndexOccurrence>();

        public void Tokenizer(string data, int siteId)
        {
            data = StopwordTool.RemoveStopwords(data);
            Porter port = new Porter();

            string[] tempTokens = data.Split(' ');


            tempTokens = tempTokens.Where(x => !string.IsNullOrEmpty(x.Trim())).ToArray();

            for (int i = 0; i < tempTokens.Length; i++)
            {
                if (tempTokens[i].Length > 5)
                {
                    tokens.Add(port.stem(tempTokens[i]));
                }
                else if (tempTokens[i].Length > 2)
                {
                    tokens.Add(tempTokens[i]);
                }

            }

            CreateIndex(siteId);
        }

        public void CreateIndex(int siteId)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (!index.ContainsKey(tokens[i]))
                {
                    index.Add(tokens[i], new IndexOccurrence(tokens[i]));
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
                item.Value.pages.OrderByDescending(x => x.Key).ToList();
            }


            tokens.Clear();
        }

        public void Ranking()
        {
            foreach (var item in index)
            {
                weightedIndex.Add(item.Key, item.Value);
            }

            foreach (var term in weightedIndex)
            {
                for (int i = 0; i < Crawler.siteId + 1; i++)
                {
                    if (term.Value.pages.ContainsKey(i))
                    {
                        term.Value.pages[i] = (1 + Math.Log10(term.Value.pages[i]));

                    }
                }
            }

            double docFreq = 0;

            foreach (var term in weightedIndex)
            {
                docFreq = term.Value.pages.Count;

                term.Value.idf = Math.Log10(Crawler.siteId / docFreq);

                for (int i = 0; i < Crawler.siteId + 1; i++)
                {
                    if (term.Value.pages.ContainsKey(i))
                    {
                        term.Value.pages[i] = term.Value.pages[i] * (Math.Log10(Crawler.siteId/docFreq));

                    }
                }
            }

        }
    }
}