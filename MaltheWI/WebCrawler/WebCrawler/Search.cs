using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    class Search
    {
        public List<SResult> results = new List<SResult>();
        private Dictionary<int, double> queryIds = new Dictionary<int, double>();
        string query = "";
        string[] searchTerms;

        /*public void StartSearch(Dictionary<string, IndexOccurrence> index, string query)
        {
            query = query.ToLower();
            searchTerms = query.Split(' ');
            bool and = false;
            //bool or = false;
            bool renewSearch = false;

            for (int i = 0; i < searchTerms.Length; i++)
            {
                
                
                if (searchTerms[i] == "and")
                {
                    and = true;
                }/*
                else if (searchTerms[i] == "or")
                {
                    or = true;
                }*/
               /* else if(index.ContainsKey(searchTerms[i]))
                {
                    foreach (var item in index[searchTerms[i]].pages)
                    {

                        //AND
                        if (and == true)
                        {
                            renewSearch = true;

                            foreach (var item2 in searchResults)
                            {
                                if (item.Key == item2.Key)
                                {
                                    queryIds.Add(item2.Key, item2.Value);
                                    
                                }

                            }
                        }/*
                        else if (or == true)
                        {

                        }*/
                       /* else
                        {
                            searchResults.Add(item.Key, item.Value);
                        }
                        


                    }

                    if (renewSearch == true)
                    {
                        searchResults.Clear();
                        renewSearch = false;
                    }
                    

                    foreach (var item3 in queryIds)
                    {
                        searchResults.Add(item3.Key, item3.Value);
                    }

                    if (and == false /*|| or == false*///)
                  /*  {
                        queryIds.Clear();
                    }
                    and = false;
                    //or = false;


                }
                

            }
            
        }*/

        public void IndexRanker(Dictionary<string, IndexOccurrence> index, string query)
        {

            query = query.ToLower();
            searchTerms = query.Split(' ');

            Dictionary<int, double> scores = new Dictionary<int, double>();
            Dictionary<int, double> lenght = new Dictionary<int, double>();

            List<double> tempVal = new List<double>();
            double tempLenght = 0;
            bool keepRunning = false;

            for (int i = 0; i < Crawler.siteId+1; i++)
            {
                foreach (var term in index)
                {
                    if (index[term.Key].pages.ContainsKey(i))
                    {
                        keepRunning = true;
                        tempVal.Add(Math.Pow(index[term.Key].pages[i], 2));
                    }
                }
                if (keepRunning == true)
                {
                    foreach (var item in tempVal)
                    {
                        tempLenght += item;
                    }
                    lenght.Add(i, Math.Sqrt(tempLenght));

                    tempVal.Clear();
                }
                keepRunning = false;
                
            }





            foreach (var term in searchTerms)
            {
                foreach (var page in index[term].pages)
                {
                    if (!scores.ContainsKey(page.Key))
                    {
                        scores.Add(page.Key, 0);
                    }
                    scores[page.Key] += page.Value;
                    
                }
            }

            for (int i = 0; i < Crawler.siteId+1; i++)
            {
                if (scores.ContainsKey(i))
                {
                    scores[i] = scores[i] / lenght[i];
                }
                
            }
                
            

            

            foreach (var item in scores)
            {
                results.Add(new SResult(item.Key, item.Value));
            }




            
            /*
            

            string[] termCollection

            for (int i = 0; i < newTerms.Count; i++)
            {

            }*/
        }

    }
}
