﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.ProductComponents
{
    class Chassis : ComputerComponents
    {
        string type;
        bool atx;
        bool miniAtx;
        bool miniItx;
        string fans;
        string brand;
        string weight;
        string height;
        string depth;
        string width;

        public override void ParseProductSpecifications(string siteData)
        {

        }

    }
}
