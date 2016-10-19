using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products;

namespace ReviewCrawler.Sites
{
    class DBConnect
    {
        private string connectionString = "server=172.25.23.57;database=crawlerdb;user=crawler;port=3306;password=Crawler23!;";
        MySqlConnection connection;

        public void DbInitialize()
        {

            int i = 0;
            connection = new MySqlConnection(connectionString);
            connection.Open();


            connection.Close();
        }

        //string date, string content, double productRating, double reviewRating, string author, int positiveCount,
        //int negativeCount, bool verifiedPurchase, bool isCriticReview, string productType, string url, string title

        public void InsertReview(Review review)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO Review" +
                      "(date,content,productRaiting,reviewRating,author,positiveCount,negativeCount,verifiedPurchase,isCriticReview,productType,url,title,maxRating)" 
                      + "VALUES(@date, @content, @productRating, @reviewRating, @author, @positiveCount,"
                      + "@negativeCount, @verifiedPurchase, @isCriticReview, @productType, @url, @title, @maxRating)", connection);
            command.Parameters.AddWithValue("@date", review.reviewDate);
            command.Parameters.AddWithValue("@content", review.content);
            command.Parameters.AddWithValue("@productRating", review.productRating);
            command.Parameters.AddWithValue("@reviewRating", review.reviewRating);
            command.Parameters.AddWithValue("@author", review.author);
            command.Parameters.AddWithValue("@positiveCount", review.reception.positive);
            command.Parameters.AddWithValue("@negativeCount", review.reception.negative);
            command.Parameters.AddWithValue("@verifiedPurchase", review.verifiedPurchase);
            command.Parameters.AddWithValue("@isCriticReview", review.isCriticReview);
            command.Parameters.AddWithValue("@productType", review.productType);
            command.Parameters.AddWithValue("@url", review.url);
            command.Parameters.AddWithValue("@title", review.title);
            command.Parameters.AddWithValue("@maxRating", review.maxRating);

            command.ExecuteNonQuery();

            int ID = GetReviewID(review.url);

            foreach (ReviewComment comment in review.comments)
            {
                InsertReviewComment(comment, ID);
            }
        }


        public void InsertReviewComment(ReviewComment comment, int ID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO ReviewComment" +
                      "(reviewID, content, rating)" +
                      "VALUES(@reviewID, @content, @rating)", connection);
            command.Parameters.AddWithValue("@reviewID", ID);
            command.Parameters.AddWithValue("@content", comment.content);
            command.Parameters.AddWithValue("@rating", comment.rating);
            command.ExecuteNonQuery();

        }

        public int GetReviewID(string url)
        {
            MySqlCommand command = new MySqlCommand("SELECT ID FROM Review WHERE url=@url", connection);
            command.Parameters.AddWithValue("@url", url);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            return (int)reader.GetValue(0);
        }

        public void InsertProduct(Product product)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Product" +
          "(description, name)" +
          "VALUES(@description, @name)", connection);
            command.Parameters.AddWithValue("@description", product.description);
            command.Parameters.AddWithValue("@content", product.name);
            command.ExecuteNonQuery();

            int RID;

            int PID = GetProductID(product.name);

            foreach (var retailer in product.retailers)
            {
                // need to check if a retailer exists here and add if not DO DODO DODO DOD OODO DO
                //insertRetailer()!! THIS ONE IS AFTER THE OTHER OBJECTIVE
                RID = GetRetailerID(retailer.name);
                 
                InsertProductRetailer();
            }

            

        }

        public void InsertProductRetailer() ///NEXT OBJECTIVE
        {

        }

        public void InsertRetailer(string retailer)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Retailer" +
                      "(name)" +
                      "VALUES(@name)", connection);
            command.Parameters.AddWithValue("@name", retailer);

            command.ExecuteNonQuery();
        }

        public int GetProductID(string name)
        {
            MySqlCommand command = new MySqlCommand("SELECT ID FROM Product WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", name);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            return (int)reader.GetValue(0);
        }

        public int GetRetailerID(string retailer)
        {
            MySqlCommand command = new MySqlCommand("SELECT ID FROM Retailer WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", retailer);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            return (int)reader.GetValue(0);
        }

        /*
        public void InsertSpeaker()
        {


            MySqlCommand command = new MySqlCommand("INSERT INTO SoundCard" +
                      "(ProductID,type,speakerSupport,socket,fullDuplex)" +
                      "VALUES(@ID, @type, @speakerSupport, @socket, @fullDuplex)", connection);
            command.Parameters.AddWithValue("@ID", pCount);
            command.Parameters.AddWithValue("@type", pType);
            command.Parameters.AddWithValue("@speakerSupport", author);
            command.Parameters.AddWithValue("@socket", content);
            command.Parameters.AddWithValue("@fullDuplex", title);
            command.ExecuteNonQuery();


        }*/

    }
}
