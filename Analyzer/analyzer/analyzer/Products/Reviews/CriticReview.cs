using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Reviews
{
    class CriticReview : Review
    {
        public CriticReview(int id, int productId, double rating, double maxRating, DateTime date, string title, string url, string category) 
                    : base(id, productId, rating, maxRating, date, title, url, category)
        {
        }
    }
}