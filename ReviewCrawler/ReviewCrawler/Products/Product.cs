using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System;

namespace ReviewCrawler.Products
{

    //HUSK at ram indeholder 2x model. fix det.
    abstract class Product
    {
        public string name;
        public string description = "";
        protected byte[][] image;
        public List<Retailer> retailers = new List<Retailer>();
        public MySqlConnection connection;

        protected abstract void AddInformation(Dictionary<string, string> productInformation);

        public void ParseProductSpecifications(string siteData, Dictionary<string, string> regexPatterns)
        {
            Dictionary<string, string> productInfo = new Dictionary<string, string>();

            MatchCollection rawProductInformation = (Regex.Matches(siteData, regexPatterns["table"], RegexOptions.Singleline));

            foreach (Match informationTable in rawProductInformation)
            {
                foreach (Match rawInformationRow in Regex.Matches(informationTable.Value, regexPatterns["spec"], RegexOptions.Singleline))
                {
                    Regex removeTags = new Regex("(<.*?>)+", RegexOptions.Singleline);

                    // - find type of row - 
                    string tempType = removeTags.Replace(Regex.Match(rawInformationRow.Value, regexPatterns["spec name"]).Value, "").Trim();

                    // - find data of row - 
                    string tempValue = removeTags.Replace(Regex.Match(rawInformationRow.Value, regexPatterns["spec value"]).Value, "").Trim();

                    productInfo.Add(tempType, tempValue);
                }
            }

            AddInformation(productInfo);
            //databasethis(this);
        }
        
        public void ParsePrice(string siteData, Dictionary<string, string> regexPatterns)
        {
            //find title of product
            name = Regex.Match(siteData, regexPatterns["title"], RegexOptions.Singleline).Value.Replace("<title>", "").Replace("- Sammenlign priser", "").Trim();

            // Find retailers and add to product
            foreach (Match oneRetailerCode in Regex.Matches(siteData, regexPatterns["all retailers"]))
            {
                Retailer tempRetailer = new Retailer();

                // Finding retailer name.
                Regex split = new Regex("\\.\\*\\?");
                string[] tags = split.Split(regexPatterns["retailer name"]);
                tempRetailer.name = Regex.Match(oneRetailerCode.Value, regexPatterns["retailer name"]).Value.Replace(tags[0], "").Replace(tags[1], "");

                if (tempRetailer.name != "")
                {
                    // Finding retailer price
                    Regex removeTags = new Regex("(<.*?>)+", RegexOptions.Singleline);
                    string tempPrice = Regex.Match(oneRetailerCode.Value, regexPatterns["retailer price"]).Value;

                    tempPrice = removeTags.Replace(tempPrice, "").Replace(".", "");
                    tempPrice = tempPrice.Replace("kr", "").Trim().Replace(",", ".");
                    tempRetailer.price = decimal.Parse(tempPrice);

                    retailers.Add(tempRetailer);
                }
            }
        }
        

        //edbpriser match title -> <h1 class=\"product-details-header\" itemprop=\"name\">.*?</h1>

        //edbpriser match alle retailers -> <div class=\"ProductDealerList\">.*?
        //edbpriser matches pris -> <td><strong>.*? kr</strong></td>
        //edbpriser matches navn -> <div class=\"vendor-name\">.*?</div>   + html remove (<REMOVE>) + trim() + TEST DET

        //edbpriser matches spec tables -> <td class=\"headline\" colspan=.*?</table>
        //edbpriser matches specs ->  <tr class=\"sec\">.*?</tr>
        //edbpriser match spec navn -> <td class=\"spec\">.*?</td>
        //edbpriser match spec value -> <td>.*?</td>

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
                    if (!RetailerExists(retailer.name))
                    {
                        InsertRetailer(retailer.name);
                    }

                    RID = GetRetailerID(retailer.name);

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


        private void InsertProductRetailer(Retailer retailer, int RID, int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Product_Retailer" +
          "(productID, retailerID, url, price)" +
          "VALUES(@productID, @retailerID, @url, @price)", connection);
            command.Parameters.AddWithValue("@productID", PID);
            command.Parameters.AddWithValue("@retailerID", RID);
            command.Parameters.AddWithValue("@url", retailer.url);
            command.Parameters.AddWithValue("@price", retailer.price);

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
