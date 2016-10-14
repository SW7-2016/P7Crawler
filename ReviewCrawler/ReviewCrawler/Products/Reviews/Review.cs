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
        public string productType;
        public string title;
        public bool isCriticReview;
        public DateTime date;
        public string content;
        public double productRating;
        public List<string> pros;
        public List<string> cons;
        public string author;
        public string url;
        public List<ReviewComment> comments;
        public ReviewReception reception;
        public double reviewRating;
        public bool verifiedPurchase;
        public double maxRating;

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
