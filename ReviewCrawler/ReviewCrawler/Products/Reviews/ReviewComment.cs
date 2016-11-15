using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.Reviews
{
    class ReviewComment
    {
        public string content;
        public double rating;

        public ReviewComment(string Content, double Rating)
        {
            content = Content;
            rating = Rating;
        }
    }
}
