using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewCrawler.Products.Reviews
{
    class Review
    {
        private string productType;
        string title;
        bool isCriticReview;
        DateTime date;
        public string content;
        double productRating;
        List<string> pros;
        List<string> cons;
        string author;
        string url;
        List<ReviewComment> comments;
        ReviewReception reception;
        double reviewRating;
        bool verifiedPurchase;

        public Review(string Url, string PType)
        {
            productType = PType;
            url = Url;
        }

        public string ProductType
        {
            get
            {
                return productType;
            }
            set
            {
                if (value == "GPU"
                    && value == "CPU"
                    && value == "PSU"
                    && value == "RAM"
                    && value == "Chassis"
                    && value == "Cooling"
                    && value == "HardDrive"
                    && value == "Motherboard"
                    && value == "SoundCard"
                    && value == "RAM/HDD")
                {
                    productType = value;
                }
                else
                {
                    Debug.WriteLine("Product type not found!");
                }
            }
        }
    }
}
