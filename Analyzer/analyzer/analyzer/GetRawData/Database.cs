using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Diagnostics;
using analyzer.Products.ProductComponents;
using analyzer.Products.Retailers;
using analyzer.Products.Reviews;

namespace analyzer.GetRawData
{
    internal class DBconnect
    {
        private readonly string connectionString =
            "server=172.25.23.57;database=crawlerdb;user=analyser;port=3306;password=Analyser23!;";

        private readonly string connectionString2 =
            "server=172.25.23.57;database=analyserdb;user=analyser;port=3306;password=Analyser23!;";

        public MySqlConnection Connection;

        public void DbInitialize(bool isCrawlerDB)
        {
            if (isCrawlerDB)
                Connection = new MySqlConnection(connectionString);
            else
                Connection = new MySqlConnection(connectionString2);
        }
        public void GetReviewInfo()
        {
            public void InsertReview(Review review)
        {
            if (!DoesReviewExist(review))
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO Review" +
                          "(reviewDate, crawlDate, content,productRating,reviewRating,author,positiveCount,negativeCount,verifiedPurchase,isCriticReview,productType,url,title,maxRating)"
                          + "VALUES(@reviewDate, @crawlDate, @content, @productRating, @reviewRating, @author, @positiveCount,"
                          + "@negativeCount, @verifiedPurchase, @isCriticReview, @productType, @url, @title, @maxRating)", connection);
                command.Parameters.AddWithValue("@reviewDate", DateToString(review.reviewDate));
                command.Parameters.AddWithValue("@crawlDate", DateToString(review.crawlDate));
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

        public void GetProductInfo()
        {
            
        }

    }
}