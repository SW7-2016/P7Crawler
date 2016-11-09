using System.Collections.Generic;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Diagnostics;

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

        public void ParseProductSpecifications(string siteData)
        {
            Dictionary<string, string> productInfo = new Dictionary<string, string>();

            string rawProductInformation = (Regex.Match(siteData, "<div class=\"product-specs\">\\s*<table>.*?</table>", RegexOptions.Singleline)).Value;

            foreach (Match rawInformationRow in Regex.Matches(rawProductInformation, "(<tr\\s*>|<tr\\s+class=\"lastRow\">).*?</tr>", RegexOptions.Singleline))
            {
                // - find type of row - 
                //Used to pass sentence so that only the information is saved.(and not multible spaces and tags)
                Regex removeLongSpaces = new Regex("(\\s){2,20}");
                //Returns from "<th scope=\"row\">" to end of line
                string tempType = Regex.Match(rawInformationRow.Value, "<th scope=\"row\">.*").Value;
                tempType = tempType.Replace("<th scope=\"row\">", "");
                tempType = tempType.Replace("</th>", "");
                tempType = removeLongSpaces.Replace(tempType, "");

                // - find data of row - 
                string tempValue = Regex.Match(rawInformationRow.Value, "<td>.*?</td>").Value;
                tempValue = tempValue.Replace("<td>", "");
                tempValue = tempValue.Replace("</td>", "");

                productInfo.Add(tempType, tempValue);
            }

            AddInformation(productInfo);
            //databasethis(this);
        }

        public void ParsePrice(string siteData)
        {
            //find title of product
            name = Regex.Match(siteData, "<title>.*? - Sammenlign priser", RegexOptions.Singleline).Value.Replace("<title>", "").Replace("- Sammenlign priser", "").Trim();

            // Find retailers and add to product
            string retailerTag = "<a rel=\"nofollow\" title=\"\" target=\"_blank\" class=\"google-analytic-retailer-data pricelink\" retailer-data=\"";

            MatchCollection retailerCode = Regex.Matches(siteData, "(" + retailerTag + "(.*?(\n)*)*<\\/a>)+");

            foreach (Match oneRetailerCode in retailerCode)//""("
            {
                if (oneRetailerCode.Value == "")
                {
                    break;
                }
                Retailer tempRetailer = new Retailer();

                // looking for name of retailer
                for (int i = retailerTag.Length + 1; i < 20 + retailerTag.Length; i++)
                {
                    if (oneRetailerCode.Value[i] == '(' || oneRetailerCode.Value[i] == '"')
                    {
                        tempRetailer.name = oneRetailerCode.Value.Substring(retailerTag.Length, i - retailerTag.Length);
                        break;
                    }
                }

                // looking for price of product
                string tempPrice = Regex.Match(oneRetailerCode.Value, "((<strong>).*?(<\\/strong>))+").Value;
                if (tempPrice != "") {
                    Regex regexHtml = new Regex("(<.*?>)+", RegexOptions.Singleline);
                    tempPrice = regexHtml.Replace(tempPrice, "").Replace(".", "");
                    tempPrice = tempPrice.Remove(0, 3).Replace(",", ".");
                    tempRetailer.price = decimal.Parse(tempPrice);
                }

                // looking for URL of retailer
                //Eneste link på siden, er et redirect link der går gennem pricerunner. 

                if (tempRetailer.name != "")
                {
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
