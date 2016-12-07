using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.Reviews
{
    class Review
    {
        public string productType;
        public string title;
        public bool isCriticReview;
        public DateTime reviewDate;
        public DateTime crawlDate;
        public string content;
        public double productRating;
        public string author;
        public string url;
        public List<ReviewComment> comments = new List<ReviewComment>();
        public int positiveReception = 0;
        public int negativeReception = 0;
        public double reviewRating;
        public bool verifiedPurchase;
        public double maxRating;
        public MySqlConnection connection;

        public Review(string Url, string PType, bool IsCriticReview)
        {
            ProductType = PType;
            url = Url;

            title = "unknown";
            isCriticReview = IsCriticReview;
            reviewDate = DateTime.Now;
            crawlDate = DateTime.Now;
            content = "";
            productRating = 0;
            author = "unknown";
            reviewRating = 0;
            verifiedPurchase = false;
        }

        public string ProductType
        {
            get { return productType; }
            set
            {
                if (value == "GPU"
                    || value == "CPU"
                    || value == "PSU"
                    || value == "RAM"
                    || value == "Chassis"
                    || value == "Cooling"
                    || value == "HardDrive"
                    || value == "Motherboard"
                    || value == "RAM/HDD"
                    || value == "HDD"
                    || value == "RAM")
                {
                    productType = value;
                }
                else
                {
                    Debug.WriteLine("Product type not found!");
                }
            }
        }

        #region Database

        public void AddReviewToDB()
        {
            if (!DoesReviewExist())
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO Review" +
                                                        "(reviewDate, crawlDate, content,productRating,reviewRating,author " +
                                                        ",positiveCount,negativeCount,verifiedPurchase,isCriticReview,productType,url,title,maxRating)"
                                                        +
                                                        "VALUES(@reviewDate, @crawlDate, @content, @productRating, @reviewRating, @author, @positiveCount,"
                                                        +
                                                        "@negativeCount, @verifiedPurchase, @isCriticReview, @productType, @url, @title, @maxRating)",
                    connection);
                command.Parameters.AddWithValue("@reviewDate", DateToString(reviewDate));
                command.Parameters.AddWithValue("@crawlDate", DateToString(crawlDate));
                command.Parameters.AddWithValue("@content", content);
                command.Parameters.AddWithValue("@productRating", productRating);
                command.Parameters.AddWithValue("@reviewRating", reviewRating);
                command.Parameters.AddWithValue("@author", author);
                command.Parameters.AddWithValue("@positiveCount", positiveReception);
                command.Parameters.AddWithValue("@negativeCount", negativeReception);
                command.Parameters.AddWithValue("@verifiedPurchase", verifiedPurchase);
                command.Parameters.AddWithValue("@isCriticReview", isCriticReview);
                command.Parameters.AddWithValue("@productType", productType);
                command.Parameters.AddWithValue("@url", url);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@maxRating", maxRating);

                command.ExecuteNonQuery();

                int ID = GetReviewID(url);

                foreach (ReviewComment comment in comments)
                {
                    InsertReviewComment(comment, ID);
                }

                ReviewCountAdd();
            }
            else
            {
                Debug.WriteLine("Review " + url + " does already exist");
            }
        }

        //For testing purposes only
        private void ReviewCountAdd()
        {
            if (ProductType == "Chassis")
            {
                MainWindow.rwCHASSIS++;
            }
            else if (ProductType == "CPU")
            {
                MainWindow.rwCPU++;
            }
            else if (ProductType == "GPU")
            {
                MainWindow.rwGPU++;
            }
            else if (ProductType == "HDD")
            {
                MainWindow.rwHDD++;
            }
            else if (ProductType == "Motherboard")
            {
                MainWindow.rwMB++;
            }
            else if (ProductType == "PSU")
            {
                MainWindow.rwPSU++;
            }
            else if (ProductType == "RAM")
            {
                MainWindow.rwRAM++;
            }

        }

        public bool DoesReviewExist()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE url=@url", connection);
            command.Parameters.AddWithValue("@url", url);

            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string DateToString(DateTime date)
        {
            return date.Year + "-" + date.Month + "-" + date.Day;
        }

        private void InsertReviewComment(ReviewComment comment, int ID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO ReviewComment" +
                                                    "(ReviewID, content, rating)" +
                                                    "VALUES(@ReviewID, @content, @rating)", connection);
            command.Parameters.AddWithValue("@ReviewID", ID);
            command.Parameters.AddWithValue("@content", comment.content);
            command.Parameters.AddWithValue("@rating", comment.rating);
            command.ExecuteNonQuery();
        }

        private int GetReviewID(string url)
        {
            MySqlCommand command = new MySqlCommand("SELECT ReviewID FROM Review WHERE url=@url", connection);
            command.Parameters.AddWithValue("@url", url);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int) reader.GetValue(0);

            reader.Close();

            return result;
        }

        #endregion
    }
}