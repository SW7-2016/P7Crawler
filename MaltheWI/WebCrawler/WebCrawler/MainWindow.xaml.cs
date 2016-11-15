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
using System.IO;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Indexer ind = new Indexer();

        public MainWindow()
        {
            InitializeComponent();

            string str1 = "do not worry about your difficulties in mathematics";

            string str2 = "do not worry about your difficulties, you can easily learn what is needed";

            

          

            

            

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Crawler crawl = new Crawler();

            List<string> seedlist = new List<string>();
            seedlist.Add("https://www.reddit.com/");
            seedlist.Add("http://www.cinemablend.com/news.php");
            seedlist.Add("https://www.geeksaresexy.net/");

            crawl.startCrawl(seedlist);

            string tString = "";
            string t1String = "";

            for (int i = 0; i < Crawler.frontQueue.Count; i++)
            {
                tString += Crawler.frontQueue.ElementAt(i).url;
                tString += "\n";
            }
            for (int i = 0; i < Crawler.checkedsites.Count; i++)
            {
                t1String += Crawler.checkedsites[i];
                t1String += "\n";
            }

            textBox.Text = tString;
            textBox1.Text = t1String;
            textBox2.Text = Crawler.frontQueue.Count.ToString();
            textBox3.Text = Crawler.backList.Count.ToString();
        }
        //save
        private void button1_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < Crawler.crawlData.Count; i++)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Malthe\Dropbox\AAU\WIData\sites\" + i + ".txt");
                file.WriteLine(Crawler.crawlData[i].url);
                file.WriteLine(Crawler.crawlData[i].content);

                file.Close();
                
            }

            

        }
        //load
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string line = "";
            int counter = 0;
            bool isFirst = true;
            Site newSite;
            string tempString = "";

            while (true)
            {
                isFirst = true;
                tempString = "";

                if (File.Exists(@"C:\Users\Malthe\Dropbox\AAU\WIData\sites\" + counter + ".txt"))
                {
                    newSite = new Site("", "");
                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Malthe\Dropbox\AAU\WIData\sites\" + counter + ".txt");

                    while ((line = file.ReadLine()) != null)
                    {
                        if (isFirst)
                        {
                            newSite.url = line;
                            isFirst = false;
                        }
                        else
                        {
                            tempString += line;
                        }
                    }
                    newSite.content = tempString;
                    newSite.id = counter;
                    Crawler.crawlData.Add(newSite);

                }
                else
                {
                    break;
                }

                counter++;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Crawler.crawlData[2].content;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            

            for (int i = 0; i < Crawler.crawlData.Count; i++)
            {
                ind.Tokenizer(Crawler.crawlData[i].content, Crawler.crawlData[i].id);
            }

            ind.Ranking();
        }

        private void search_bt_Click(object sender, RoutedEventArgs e)
        {
            Search newSearch = new Search();


            //newSearch.StartSearch(ind.weightedIndex, search_tb.Text);

            newSearch.IndexRanker(ind.weightedIndex, search_tb.Text);

            var newResult = newSearch.results.OrderByDescending(x => x.score);

            string temp = "";

            foreach (var item in newResult)
            {
                temp += item.id + " " + item.score + "\n";
            }
                
                

            search_results.Text = temp;
        }
    }
}
