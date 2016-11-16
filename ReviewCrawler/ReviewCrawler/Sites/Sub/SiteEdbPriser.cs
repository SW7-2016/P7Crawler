using ReviewCrawler.Helpers;
using ReviewCrawler.Products;
using ReviewCrawler.Products.ProductComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites.Sub
{
    class SiteEdbPriser : ProductSite
    {
        public SiteEdbPriser()
        {
            domainUrl = "http://www.edbpriser.dk";
            searchQueue.Enqueue(new QueueElement("http://www.edbpriser.dk/hardware/ram.aspx?count=500&sort=Popularity&rlm=List", ""));
            //searchQueue.Enqueue(new QueueElement("http://www.edbpriser.dk/hardware/bundkort.aspx?count=500&sort=Popularity&rlm=List", ""));
            //searchQueue.Enqueue(new QueueElement("http://www.edbpriser.dk/hardware/harddisk.aspx?count=500&sort=Popularity&rlm=List", ""));
            //searchQueue.Enqueue(new QueueElement("http://www.edbpriser.dk/hardware/processor.aspx?count=500&sort=Popularity&rlm=List", ""));
            //searchQueue.Enqueue(new QueueElement("http://www.edbpriser.dk/hardware/grafikkort.aspx?count=500&sort=Popularity&rlm=List", ""));
            //searchQueue.Enqueue(new QueueElement("http://www.edbpriser.dk/hardware/kabinet.aspx?count=500&sort=Popularity&rlm=List", ""));
            //searchQueue.Enqueue(new QueueElement("http://www.edbpriser.dk/hardware/stroemforsyning.aspx?count=500&sort=Popularity&rlm=List", ""));
        }


        public override void CrawlPage(string siteData, string sQueueData)
        {

            //finding next page of the review overview
            Match nextPage = Regex.Match(siteData, "<li class=\"next\"><a href=\".*?\"");
            //If there is found a next page, then add it to the queue.
            if (nextPage.Value != "")
            {
                string tempPage = nextPage.Value.Replace("<li class=\"next\"><a href=\"", "").Replace("\"", "");
                searchQueue.Enqueue(new QueueElement(domainUrl + tempPage, ""));
            }

            //Finding the product links from a page.
            MatchCollection newProducts = Regex.Matches(siteData, "<a class=\"link-action\" href=\".*?\"");

            //Adding all the matches of specifik products
            foreach (Match link in newProducts)
            {
                itemQueue.Enqueue(new QueueElement(domainUrl + link.Value.Replace("<a class=\"link-action\" href=\"", "").Replace("\"", ""), ""));
            }
        }

        public override bool Parse(string siteData, string sQueueData)
        {
            product = null;

            if (currentSite.Contains("/grafikkort"))
            {
                //product is a "GPU"
                product = new GPU();
            }
            else if (currentSite.Contains("/cpu"))
            {
                //product is a "CPU"
                product = new CPU();
            }
            else if (currentSite.Contains("/bundkort"))
            {
                //product is a "Motherboard"
                product = new Motherboard();
            }
            else if (currentSite.Contains("/harddisk"))
            {
                //product is a "HardDrive"
                product = new HardDrive();
            }
            else if (currentSite.Contains("/kabinet"))
            {
                //product is a "Chassis"
                product = new Chassis();
            }
            else if (currentSite.Contains("/stroemforsyning"))
            {
                //product is a "PSU"
                product = new PSU();
            }
            else if (currentSite.Contains("/ram"))
            {
                //product is a "RAM"
                product = new RAM();
            }
            else
            {
                throw new FormatException("Pricerunner - couldnt determine product type", null);
            }

            //We have created our product, now we fill the data
            ProductPriceRegexes edbPriserParsePrice = new ProductPriceRegexes(
                "<h1 class=\"product-details-header\" itemprop=\"name\">.*?</h1>",
                "<div class=\"vendor\">.*?</tr>",
                "<td><strong>.*? kr</strong></td>",
                "<div class=\"vendor-name\">.*?</div>");

            ProductSpecRegexes edbPriserParseSpecs = new ProductSpecRegexes(
                "<td class=\"headline\" colspan=.*?</table>",
                "<tr.*?</tr>",
                "<td class=\"spec\">.*?</td>",
                "<td>.*?</td>");

            product.ParsePrice(siteData, edbPriserParsePrice);
            product.ParseProductSpecifications(siteData, edbPriserParseSpecs);
            
            return true;
        }
    }
}

