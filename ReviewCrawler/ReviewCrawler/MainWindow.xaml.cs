using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Threading;
using System.Windows.Threading;

namespace ReviewCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool ContinueCrawling = true;
        Crawler crawler = new Crawler();
        //GUI TEST PURPOSES ONLY
        public static int rwGPU = 0;
        public static int rwCPU = 0;
        public static int rwMB = 0;
        public static int rwRAM = 0;
        public static int rwHDD = 0;
        public static int rwPSU = 0;
        public static int rwCHASSIS = 0;
        public static int pdGPU = 0;
        public static int pdCPU = 0;
        public static int pdMB = 0;
        public static int pdRAM = 0;
        public static int pdHDD = 0;
        public static int pdPSU = 0;
        public static int pdCHASSIS = 0;
        public static int guru3d = 0;
        public static int edbpriser = 0;
        public static int pricerunner = 0;
        public static int amazon = 0;
        public static int techpowerup =0;
        public static int computershopper = 0;
        //

        public MainWindow()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Start();

            InitializeComponent();
        }

        private void crawl_bt_Click(object sender, RoutedEventArgs e)
        {
            crawler.AddHosts();

            Debug.WriteLine("Starting thread");
            Thread crawlerThread = new Thread(crawler.StartCrawlCycle);
            crawlerThread.Start("");
            Debug.WriteLine("Thread started and crawler is now running.");
        }

        private void UpdateGUI()
        {
            amazon_tb.Text = "Amazon: " + amazon;
            edbpriser_tb.Text = "EdbPriser: " + edbpriser;
            pricerunner_tb.Text = "PriceRunner: " + pricerunner;
            guru3d_tb.Text = "Guru3d: " + guru3d;
            techpowerup_tb.Text = "TechPU: " + techpowerup;
            computerShopper_tb.Text = "CompSho: " + computershopper;

            rwCPU_tb.Text = "rwCPU: " + rwCPU;
            rwGPU_tb.Text = "rwGPU: " + rwGPU;
            rwPSU_tb.Text = "rwPSU: " + rwPSU;
            rwHDD_tb.Text = "rwHDD: " + rwHDD;
            rwRAM_tb.Text = "rwRAM: " + rwRAM;
            rwMB_tb.Text = "rwMB: " + rwMB;
            rwChassis_tb.Text = "rwChas: " + rwCHASSIS;

            pdCPU_tb.Text = "pdCPU: " + pdCPU;
            pdGPU_tb.Text = "pdGPU: " + pdGPU;
            pdPSU_tb.Text = "pdPSU: " + pdPSU;
            pdHDD_tb.Text = "pdHDD: " + pdHDD;
            pdRAM_tb.Text = "pdRAM: " + pdRAM;
            pdMB_tb.Text = "pdMB: " + pdMB;
            pdChassis_tb.Text = "pdChas: " + pdCHASSIS;

            totA_tb.Text = "Total: " + (amazon + edbpriser + pricerunner + guru3d + techpowerup + computershopper);
            totPD_tb.Text = "Total pd: " + (pdCHASSIS + pdCPU + pdGPU + pdHDD + pdMB + pdPSU + pdRAM);
            totRW_tb.Text = "Total rw: " + (rwCHASSIS + rwCPU + rwGPU + rwHDD + rwMB + rwPSU + rwRAM);
        }

        private void stopCrawl_bt_Click(object sender, RoutedEventArgs e)
        {

            ContinueCrawling = false;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UpdateGUI();
        }
    }
}

