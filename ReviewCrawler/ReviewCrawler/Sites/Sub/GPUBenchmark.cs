using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewCrawler.Helpers;

namespace ReviewCrawler.Sites.Sub
{
    class GPUBenchmark : ReviewSite
    {
        public GPUBenchmark()
        {
            domainUrl = "http://www.videocardbenchmark.net/";
            searchQueue.Enqueue(new QueueElement("http://www.videocardbenchmark.net/high_end_gpus.html", ""));
            searchQueue.Enqueue(new QueueElement("http://www.videocardbenchmark.net/mid_range_gpus.html", ""));
            searchQueue.Enqueue(new QueueElement("http://www.videocardbenchmark.net/midlow_range_gpus.html", ""));
        }

        public override void CrawlPage(string siteData, string sQueueData)
        {
            string[] articleLinks = regexMatches(siteData, "class=\"chart\">    <a href=\"", "\">");

            for (int i = 0; i < articleLinks.Length - 1; i++)
            {
                itemQueue.Enqueue(new QueueElement(domainUrl + articleLinks[i], ""));
            }
        }

        public override bool Parse(string siteData, string sQueueData)
        {
            BenchmarkElement benchItem = new BenchmarkElement();

            benchItem.title = regexMatch(siteData, "<span class=\"cpuname\">", "</span>");
            benchItem.date = GetReleaseDate(siteData);
            benchItem.price = GetPrice(siteData);
            benchItem.productType = "GPU";
            benchItem.score = GetScore(siteData);
            benchItem.scoreMoney = GetScoreMoney(siteData);

            bList.Add(benchItem);
            return false;
        }

        private double GetScore(string data)
        {
            string result = regexMatch(data, "red;\">", "</span>");

            if (result != "")
            {
                return double.Parse(result);
            }
            else
            {
                return -1;
            }
        }

        private double GetScoreMoney(string data)
        {
            string tempPrice = regexMatch(data, "Price:</span>&nbsp;&nbsp;", "&nbsp;&nbsp;");
            double result;
            if (tempPrice == "NA")
            {
                return -1;
            }
            if (tempPrice.Contains("."))
            {
                string[] tempPriceSplit = tempPrice.Split('.');
                result = double.Parse(tempPriceSplit[0]) + (double.Parse(tempPriceSplit[1])/10);
            }
            else
            {
                result = double.Parse(tempPrice);
            }


            return result;
        }

        private double GetPrice(string data)
        {
            string tempPrice = regexMatch(data, "Last Price Change", "<br />");
            tempPrice = regexMatch(tempPrice, "\\$", "USD").Replace("$", "");
            double result;
            if (tempPrice == "")
            {
                return -1;
            }
            if (tempPrice.Contains("."))
            {
                string[] tempPriceSplit = tempPrice.Split('.');
                result = double.Parse(tempPriceSplit[0]) + (double.Parse(tempPriceSplit[1])/10);
            }
            else
            {
                result = double.Parse(tempPrice);
            }


            return result;
        }

        private DateTime GetReleaseDate(string data)
        {
            string tempDate = regexMatch(data, "Videocard First Benchmarked:</span>&nbsp;&nbsp;", "<br>");

            string[] splitDate = tempDate.Split('-');

            if (splitDate.Length == 3)
            {
                return new DateTime(int.Parse(splitDate[0]), int.Parse(splitDate[1]), int.Parse(splitDate[2]));
            }
            else
            {
                //throw new ArgumentOutOfRangeException();
                return new DateTime(1900, 1, 1);
            }
        }

        public override string GetProductType(string data)
        {
            return "unknown";
        }
    }
}