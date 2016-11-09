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
using ReviewCrawler.Sites;
using System.IO;
using ReviewCrawler.Sites.Sub;

namespace ReviewCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Crawler crawl = new Crawler();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void crawl_bt_Click(object sender, RoutedEventArgs e)
        {

            crawl.AddHosts();
            crawl.StartCrawl();
        }

        private void sendDataTest_bt_Click(object sender, RoutedEventArgs e)
        {

        }

        private void test_bt_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            using (StreamReader inputFile = new StreamReader(@"C:\Users\Malthe\Desktop\Damp.txt"))
            {

                while (!inputFile.EndOfStream)
                {

                    result += inputFile.ReadLine();

                }
            }

             SiteGuru3d dub = new SiteGuru3d();

            dub.removeTagsFromReview(result);
        }
    }
}
