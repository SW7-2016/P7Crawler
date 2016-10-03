using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WI.objects;
using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WI
{
    public partial class MainWindow : Window
    {
        //Settings
            //No. of sites the crawler max crawls
        const int MAX_VISITS = 100;
            //Save folders for files
        public static string PATH_TOKENIZED = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\WI\Tokenized\");
        public static string PATH_RAW = (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\WI\Raw\");

        //Variables for thread communication
        public static int crawlerProgress = -1;
        public static int tokenizerProgress = -1;

        //Variables for GUI communication
        public static string CurUrl = "";
        public static int TokenizedPages = 0;
        public static int IndexedTokens = 0;
        Ranker rankedIndex = new objects.Ranker(new Dictionary<string, IndexToken>());

        //Shared for crawler (NEED REMOVAL?)
        public static Indexer indexer = new Indexer();
        public static List<Host> hosts = new List<Host>();
        public static Queue<Site> urls = new Queue<Site>();
        public static List<Site> allUrls = new List<Site>();

        //Threads 
        Thread Crawler = new Thread(WI.MainWindow.crawling);
        Thread Tokenizer = new Thread(WI.MainWindow.tokenizer);
        Thread Indexing = new Thread(WI.MainWindow.indexing);
        Thread Ranker = new Thread(WI.MainWindow.indexing);

        public MainWindow()
        {
            InitializeComponent();
        }

        //Creating the index from all the files from the tokenizer.
        public static void indexing(object data)
        {
            for (int i = 0; MAX_VISITS > i;)
            {
                if (tokenizerProgress >= i)
                {
                    if (File.Exists(PATH_TOKENIZED + i + ".txt"))
                    {
                        StreamReader sr = new StreamReader(PATH_TOKENIZED + i + ".txt");
                        string[] tokens = sr.ReadToEnd().Split('\n');
                        indexer.CreateIndex(i, tokens.ToList());
                    }
                    i++;
                }
            }
        }

        //Makes the Raw file from crawler, to a tokenized file. 
        public static void tokenizer(object data)
        {
            Porter porter = new Porter();
            for (int i = 0; MAX_VISITS > i;)
            {
                if (crawlerProgress >= i)
                {

                    if (File.Exists(PATH_RAW + i + ".txt"))
                    {
                        StreamReader sr = new StreamReader(PATH_RAW + i + ".txt");
                        string url = sr.ReadLine();
                        string rawToTokens = sr.ReadToEnd();
                        
                        rawToTokens = Tools.RemoveHtmlTags(rawToTokens);
                        string[] tokenStream = Tools.RemoveStopwords(rawToTokens).Split('\n');

                        Porter stem = new Porter();

                        using (StreamWriter outputFile = new StreamWriter(PATH_TOKENIZED + i + ".txt"))
                        {
                            outputFile.WriteLine(url);

                            foreach (string fakeToken in tokenStream)
                            {
                                foreach (string token in fakeToken.Trim().Split(' '))
                                {
                                    if (token != "" && token.Length > 1)
                                        outputFile.WriteLine(stem.stem(token));
                                }
                            }
                        }
                        sr.Close();
                    }
                    TokenizedPages++;
                    tokenizerProgress = i;
                    i++;
                }
            }
        }

        //Handles crawling
        public static void crawling(object data)
        {
            //Creating init frontier.
            Site.Add("https://www.reddit.com");

            //Creating web client, so that the program can fetch websites.
            WebClient webClient = new WebClient();
            webClient.Proxy = null;

            for (int i = 0; urls.Count() > 0 && MAX_VISITS > i; i++, crawlerProgress = i - 1)
            {
                //fetching the new side.
                Site curUrl = urls.Dequeue();

                //Check if rules(robots.txt) are loaded, otherwise load them.
                if (!curUrl.parent.robots)
                    curUrl.parent.getRobotsTxt();

                //Setting the Global variable, to show what url is worked on.
                CurUrl = curUrl.url;

                //Processing site if the site is allowed by the Robots.txt (explicit fairness)
                if (curUrl.parent.amIAllowed(curUrl.url))
                {
                    //ensuring implicit fairness, max one site load pr 2 seconds.
                    if (!(DateTime.Now > curUrl.parent.timeStamp.AddSeconds(2)))
                    {
                        urls.Enqueue(curUrl);
                        i--;
                        continue;
                    }

                    //reading current site to byte array.
                    byte[] text = null;
                    try
                    {
                        text = webClient.DownloadData(curUrl.url);
                    }
                    catch
                    {
                        continue;
                    }

                    //First thing after reading the site, is setting the timestamp for futur fairness.
                    curUrl.parent.timeStamp = DateTime.Now;
                    //Making site readable for diffrent processing
                    string webData = Encoding.UTF8.GetString(text);
                    //Data to futur Tokenizing
                    string[] toOutput = webData.ToLower().Split('\n');
                    //Data split in lines, to make faster search for links
                    string[] lines = webData.ToLower().Split(' ');

                    //Write the site to a file. This file is then used for tokenizing.
                    using (StreamWriter outputFile = new StreamWriter(PATH_RAW + i + ".txt"))
                    {
                        //Save the URL, in top of document.
                        outputFile.WriteLine(CurUrl);

                        foreach (string line in toOutput)
                            outputFile.WriteLine(line);
                    }

                    //Is true when "http" is found
                    bool link = false;
                    //Placeholder for the new url, as it is read char for char.
                    string newLinkUrl = "";

                    //Loop to look through each line of the raw site.
                    for (int k = 0; k < lines.Count(); k++)
                    {
                        //If the line contains "http", we need to search that line.
                        if (lines[k].Contains("http"))
                        {
                            //Loops to go through every char in line, that contains http.
                            for (int j = 0; j < lines[k].Count(); j++)
                            {

                                //When the char: " is read, we know the link is at end, and we save the link
                                if (lines[k][j] == '"' && link)
                                {
                                    link = false;
                                    //Add site to frontier (Some sites are sorted out under the "Site.add()")
                                    Site.Add(newLinkUrl);
                                }

                                //Reading the link as long as it has not ended.
                                if (link)
                                {
                                    newLinkUrl += lines[k][j];
                                }

                                //Finding the char: ",  if it is found then we search for link.
                                if (lines[k][j] == '"' && !link)
                                {
                                    //resetting placeholder
                                    newLinkUrl = "";
                                    string temp = "";

                                    //We read the first 4 chars, if the line is long enough.
                                    if (lines[k].Count() - j > 5)
                                    {
                                        for (int n = 1; n < 5; n++)
                                        {

                                            temp += lines[k][j + n];
                                        }
                                    }
                                    //If the read 4 chars are "http", we know the following chars are a link.
                                    if (temp.ToLower() == "http")
                                    {
                                        link = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Grid_LayoutUpdated(object sender, EventArgs e)
        {
            //Updates crawler info.
           // lQueue.Content = urls.Count();
           // lHosts.Content = hosts.Count();
           // lCompleted.Content = (allUrls.Count() - urls.Count());
            //lCurSite.Content = CurUrl;
            //lTokenCount.Content = TokenizedPages;
            //lIndexedCount.Content = IndexedTokens;
        }

        private void LookUpToken_Click(object sender, RoutedEventArgs e)
        {
            OutputIndexer.Text = "";
            string token = InputIndexer.Text.ToLower();
            Porter stem = new Porter();
            Dictionary<int, int> results = indexer.FindToken(stem.stem(token));

            if (results != null)
            {
                string textResult = "";

                foreach (KeyValuePair<int, int> result in results)
                {
                    textResult += "occurrences: " + result.Value + "  Site number: " + result.Key + "\n";
                }

                OutputIndexer.Text = textResult;
            }
        }

        //Start Tokenizer button
        private void BTokenizer_Click(object sender, RoutedEventArgs e)
        {
            Tokenizer.Start(2);
            BTokenizer.IsEnabled = false;
        }
        
        private void BRanker_Click(object sender, RoutedEventArgs e)
        {
            rankedIndex = new Ranker(indexer.index);
        }

        //Start Crawler button
        private void BCrawler_Click(object sender, RoutedEventArgs e)
        {
            Crawler.Start(1);
            BCrawler.IsEnabled = false;
        }

        //Start Indexing button
        private void BIndexer_Click(object sender, RoutedEventArgs e)
        {
            Indexing.Start(3);
            BIndexer.IsEnabled = false;
        }

        //Search and rank button
        private void BSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchOutput.Text = "";
            Porter stem = new Porter();
            string[] query = Tools.RemoveStopwords(InputSearch.Text).Trim().Split(' ');
            List<KeyValuePair<int, double>> result = new List<KeyValuePair<int, double>>();
            double tempDotProduct = 0;
            double[] tempLenght = {1, 1};
            Dictionary<int, double[]> sites = new Dictionary<int, double[]>();

            List<RankedToken> tokens = new List<RankedToken>();

            foreach (string word in query)
            {
                if (rankedIndex.index.ContainsKey(stem.stem(word.ToLower()) + '\r'))
                    tokens.Add(rankedIndex.index[stem.stem(word.ToLower()) + '\r']);
            }

            for (int i = 0; i < tokens.Count(); i++)
            {
                foreach (KeyValuePair<int, double> site in tokens[i].TfIdf)
                {
                    if (sites.ContainsKey(site.Key))
                    {
                        sites[site.Key][i] = site.Value;
                    }
                    else
                    {
                        sites.Add(site.Key, new double[query.Count()]);
                        sites[site.Key][i] = site.Value;
                    }
                }
            }

            int tempInt;
            List<int> removeId = new List<int>();

            foreach (KeyValuePair<int, double[]> site in sites)
            {
                tempInt = 0;
                for (int i = 0; i < site.Value.Length; i++)
                {
                    if (site.Value[i] != 0)
                    {
                        tempInt++;
                    }
                }
                if (site.Value.Length / tempInt > 3/2)
                {
                    removeId.Add(site.Key);
                }
            }

            foreach (int remove in removeId)
            {
                sites.Remove(remove);
            }

            foreach (KeyValuePair<int, double[]> site in sites)
            {
                tempDotProduct = 0;
                tempLenght[0] = 1;
                tempLenght[1] = 1;

                for (int i = 0; i < query.Count(); i++)
                {
                    tempDotProduct = tempDotProduct + (site.Value[i] * 1);
                    tempLenght[0] = tempLenght[0] + 1;
                    tempLenght[1] = tempLenght[1] + Math.Pow(site.Value[i], 2);

                }

                result.Add(new KeyValuePair<int, double>(site.Key, (Math.PI / 180) * (
                    Math.Acos((tempDotProduct) / (Math.Sqrt(tempLenght[0]) * Math.Sqrt(tempLenght[1]))))));
            }

            result = result.OrderBy(x => x.Value).ToList();

            if (result.Count() > 0)
            {
                string tempResult = "";
                for (int i = 0; i < (result.Count() > 10 ? 10 : result.Count()); i++)
                {
                    tempResult += "   site: " + result.ElementAt(i).Key + " vinkel: " + Math.Round((result.ElementAt(i).Value) * 10000) / 100 + "     ";

                    using (StreamReader FileReader = new StreamReader(PATH_TOKENIZED + result.ElementAt(i).Key + ".txt"))
                    {
                        tempResult += FileReader.ReadLine() + '\n';
                    }
                }
                SearchOutput.Text = tempResult;
            }
        }
    }
}
