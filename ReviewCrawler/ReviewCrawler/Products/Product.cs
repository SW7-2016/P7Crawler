using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System;
using ReviewCrawler.Helpers;

namespace ReviewCrawler.Products
{

    //HUSK at ram indeholder 2x model. fix det.
    abstract class Product
    {
        public string name;
        public string description = "";
        protected byte[][] image;
        public Dictionary<string, decimal> retailers = new Dictionary<string, decimal>();
        public MySqlConnection connection;

        protected abstract void AddInformation(List<string[]> productInformation);

        public void ParseProductSpecifications(string siteData, ProductSpecRegexes regexPatterns)
        {
            List<string[]> productInfo = new List<string[]>();

            MatchCollection rawProductInformation = (Regex.Matches(siteData, regexPatterns.specTablePattern, RegexOptions.Singleline));

            int tableNumber = -1;

            foreach (Match informationTable in rawProductInformation)
            {
                tableNumber++;

                foreach (Match rawInformationRow in Regex.Matches(informationTable.Value, regexPatterns.specRowPattern, RegexOptions.Singleline))
                {
                    Regex removeTags = new Regex("(<.*?>)+", RegexOptions.Singleline);

                    // - find type of row - 
                    string tempType = removeTags.Replace(Regex.Match(rawInformationRow.Value, regexPatterns.rowNamePattern).Value, "").Trim();

                    // - find data of row - 
                    string tempValue = removeTags.Replace(Regex.Match(rawInformationRow.Value, regexPatterns.rowValuePattern).Value, "").Trim();

                    if (tempType != "" && tempValue != "")
                    {
                        productInfo.Add(new string[3] { tempType.ToLower(), tempValue , tableNumber.ToString()});
                    }
                }
            }

            AddInformation(productInfo);
        }
        
        public void ParsePrice(string siteData, ProductPriceRegexes regexPatterns)
        {
            Regex removeTags = new Regex("(<.*?>)+", RegexOptions.Singleline);

            //Find title of product
            name = removeTags.Replace(Regex.Match(siteData, regexPatterns.productTitlePattern, RegexOptions.Singleline).Value, "");

            //Remove tags is not enough in this case
            if (name.Contains("- Sammenlign priser"))
            {
                name = name.Replace("- Sammenlign priser", "").Trim();
            }

            // Find retailers and add to product
            foreach (Match oneRetailerCode in Regex.Matches(siteData, regexPatterns.allRetailersPattern, RegexOptions.Singleline))
            {
                
                decimal tempPrice = 0;
                string tempName = "";

                // Finding retailer name.
                Regex split = new Regex("\\.\\*\\?");
                string[] tags = split.Split(regexPatterns.retailerNamePattern);
                tempName = removeTags.Replace(Regex.Match(oneRetailerCode.Value, regexPatterns.retailerNamePattern, RegexOptions.Singleline).Value.Replace(tags[0], "").Replace(tags[1], ""), "").Trim();

                if (tempName != "")
                {
                    // Finding retailer price
                    string tempStrPrice = Regex.Match(oneRetailerCode.Value, regexPatterns.retailerPricePattern).Value;

                    tempStrPrice = removeTags.Replace(tempStrPrice, "").Replace(".", "");
                    tempStrPrice = tempStrPrice.Replace("kr", "").Trim().Replace(",", ".");
                    tempPrice = decimal.Parse(tempStrPrice);

                    if (retailers.ContainsKey(tempName))
                    {
                        retailers.Add(tempName, tempPrice);
                    }
                }
            }
        }
        #region  Datebase

        public abstract void InsertComponentToDB(int PID);

        public void AddProductToDB()
        {
            if (!DoesProductExist())
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO Product" +
              "(description, name)" +
              "VALUES(@description, @name)", connection);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();

                int RID;

                int PID = GetProductID();


                InsertComponentToDB(PID);


                foreach (var retailer in retailers)
                {
                    if (!RetailerExists(retailer.Key))
                    {
                        InsertRetailer(retailer.Key);
                    }

                    RID = GetRetailerID(retailer.Key);

                    InsertProductRetailer(retailer, RID, PID);
                }
            }
            else
            {
                Debug.WriteLine("product " + name + " does already exist");
            }
        }

        public bool DoesProductExist()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Product WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", name);

            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        private void InsertProductRetailer(KeyValuePair<string, decimal> retailer, int RID, int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Product_Retailer" +
          "(productID, retailerID, url, price)" +
          "VALUES(@productID, @retailerID, @url, @price)", connection);
            command.Parameters.AddWithValue("@productID", PID);
            command.Parameters.AddWithValue("@retailerID", RID);
            command.Parameters.AddWithValue("@url", retailer.Key);
            command.Parameters.AddWithValue("@price", retailer.Value);

            command.ExecuteNonQuery();
        }

        private void InsertRetailer(string retailer)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Retailer" +
                      "(name)" +
                      "VALUES(@name)", connection);
            command.Parameters.AddWithValue("@name", retailer);

            command.ExecuteNonQuery();
        }

        private int GetProductID()
        {
            MySqlCommand command = new MySqlCommand("SELECT ProductID FROM Product WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", name);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)(reader.GetValue(0));

            reader.Close();

            return result;
        }

        private int GetRetailerID(string retailer)
        {
            MySqlCommand command = new MySqlCommand("SELECT RetailerID FROM Retailer WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", retailer);
            int result = 0;
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                result = (int)(reader.GetValue(0));
            }


            reader.Close();

            return result;
        }

        private bool RetailerExists(string retailer)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Retailer WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", retailer);

            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }


        }

        #endregion
    }
}
