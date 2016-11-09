using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Reviews
{
    internal abstract class Review
    {
        private string _category;
        private List<string> _pros;
        private List<string> _cons;
        private List<ReviewComment> _comments = new List<ReviewComment>();
        private ReviewReception _reception = new ReviewReception();

        protected Review(int id, int productId, double rating, double maxRating, DateTime date, string title,  string url, string category)
        {
            Id = id;
            _category = category;
            Url = url;
            PositiveReception = 0;
            NegativeReception = 0;
            Title = "unknown";
            ReviewDate = DateTime.Now;
            ProductRating = 0;
            MaxRating = 0;
            Author = "unknown";
        }


        public int Id { get; }
        public int PositiveReception { get; }
        public int NegativeReception { get; }
        public double ProductRating { get; }
        public double MaxRating { get; }
        public DateTime ReviewDate { get; }
        public string Url { get; }
        public string Title { get; }
        public string Author { get; }
        public string Category
        {
            get { return _category; }
            set
            {
                if ((value == "GPU")
                    || (value == "CPU")
                    || (value == "PSU")
                    || (value == "RAM")
                    || (value == "Chassis")
                    || (value == "Cooling")
                    || (value == "HardDrive")
                    || (value == "Motherboard")
                    || (value == "SoundCard")
                    || (value == "RAM/HDD"))
                    _category = value;
                else
                    Debug.WriteLine("Product type not found!");
            }
        }

    }
}