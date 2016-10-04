using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.Reviews
{
    class Review
    {
        DateTime date;
        string content;
        double productRating;
        List<string> pros;
        List<string> cons;
        string author;
        Uri url;
        List<ReviewComment> comments;
        ReviewReception reception;
        double reviewRating;
        Boolean verifiedPurchase;
    }
}
