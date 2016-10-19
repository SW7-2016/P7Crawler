using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Sites
{
    class DBConnect
    {
        private string connectionString = "server=172.25.23.57;database=crawlerdb;user=crawler;port=3306;password=Crawler23!;";
        MySqlConnection connection;

        public void DbInitialize()
        {
    

            connection = new MySqlConnection(connectionString);
            connection.Open();
            
            //InsertReview(connection);

            connection.Close();
        }

        //productRaiting -> productRating

        public void InsertReview(string date, string content, double productRating, double reviewRating, string author, int positiveCount,
            int negativeCount, bool verifiedPurchase, bool isCriticReview, string productType, string url, string title)
        {
            

            MySqlCommand command = new MySqlCommand("INSERT INTO Review" +
                      "(date,content,productRaiting,reviewRating,author,positiveCount,negativeCount,verifiedPurchase,isCriticReview,productType,url,title)" 
                      + "VALUES(@date, @content, @productRaiting, @reviewRating, @author, @positiveCount,"
                      + "@negativeCount, @verifiedPurchase, @isCriticReview, @productType, @url, @title)",connection);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@content", content);
            command.Parameters.AddWithValue("@productRaiting", productRating);
            command.Parameters.AddWithValue("@reviewRating", reviewRating);
            command.Parameters.AddWithValue("@author", author);
            command.Parameters.AddWithValue("@positiveCount", positiveCount);
            command.Parameters.AddWithValue("@negativeCount", negativeCount);
            command.Parameters.AddWithValue("@verifiedPurchase", verifiedPurchase);
            command.Parameters.AddWithValue("@isCriticReview", isCriticReview);
            command.Parameters.AddWithValue("@productType", productType);
            command.Parameters.AddWithValue("@url", url);
            command.Parameters.AddWithValue("@title", title);
            command.ExecuteNonQuery();


        }

        public void InsertReviewComment(string content, double rating)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO ReviewComment" +
                      "(content, rating)" +
                      "VALUES(@content, @rating)", connection);
            command.Parameters.AddWithValue("@content", content);
            command.Parameters.AddWithValue("@rating", rating);
            command.ExecuteNonQuery();

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
