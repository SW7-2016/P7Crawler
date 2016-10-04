using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;

namespace ReviewCrawler.Products
{
    abstract class Product
    {
        string name;
        string description;
        decimal price;
        byte[][] image;
        List<Retailer> retailers;
        List<Review> criticReviews;
        List<Review> userReviews;
    }
}
