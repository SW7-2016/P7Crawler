using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Reviews
{
    class ReviewComment
    {
        private int _reviewId;
        private string _content;
        private double _rating;

        public ReviewComment(int id, int reviewId, string content, double rating)
        {
            Id = id;
            ReviewId = reviewId;
            Content = content;
            Rating = rating;
        }

        public int Id { get; }
        public int ReviewId { get; }
        public string Content { get; }
        public double Rating { get; }

    }
}


