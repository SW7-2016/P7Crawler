﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ReviewCrawler.Products;

namespace ReviewCrawler.Sites
{
    abstract class ProductSite : Site
    {
        public Product product;

        public override abstract bool Parse(string siteData);
        public override abstract void CrawlPage(string siteData);
        public override abstract string GetSiteKey(string url);

        public override void AddItemToDatabase(MySqlConnection connection)
        {
            product.connection = connection; 
            product.AddProductToDB();
        }
    }
}
