using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Helpers
{
    class ProductSpecRegexes
    {
        public string specTablePattern;
        public string specRowPattern;
        public string rowNamePattern;
        public string rowValuePattern;

        public ProductSpecRegexes(string SpecTablePattern, string SpecRowPattern, string RowNamePattern, string RowValuePattern)
        {
            specTablePattern = SpecTablePattern;
            specRowPattern = SpecRowPattern;
            rowNamePattern = RowNamePattern;
            rowValuePattern = RowValuePattern;
        }
    }
}
