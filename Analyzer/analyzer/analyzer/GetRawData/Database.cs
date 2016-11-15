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
    class DBConnect
    {
        /*
        MySqlCommand command = new MySqlCommand("SELECT * FROM CPU WHERE url=@url", connection);
        command.Parameters.AddWithValue("@url", url);

            MySqlDataReader reader = command.ExecuteReader();

        reader.Read();

            int result = (int)reader.GetValue(0);

        reader.Close();*/
        private readonly string connectionString = "server=172.25.23.57;database=crawlerdb;user=analyser;port=3306;password=Analyser23!;";
        private readonly string connectionString2 = "server=172.25.23.57;database=analyserdb;user=analyser;port=3306;password=Analyser23!;";

        public MySqlConnection connection;

        public void DbInitialize(bool isCrawlerDb)
        {
            if (isCrawlerDb)
                connection = new MySqlConnection(connectionString);
            else
                connection = new MySqlConnection(connectionString2);
        }

        public List<Motherboard> GetMotherboardData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Motherboard", connection);
            int i = 0;
            List<Motherboard> result = new List<Motherboard>();
        //command.Parameters.AddWithValue("@url", "");

        MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                Motherboard row = new Motherboard("Motherboard", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2], 
                                                (int)tempResult[3], (string)tempResult[4], reader.GetBoolean(5), reader.GetBoolean(6),
                                                reader.GetBoolean(7), reader.GetBoolean(8), reader.GetBoolean(9), (int)tempResult[10], 
                                                (int)tempResult[11], (string)tempResult[12], reader.GetBoolean(13), (string)tempResult[14]);
                                                

                result.Add(row);
                
                    /*new Motherboard(tempResult[0], tempResult[1], tempResult[2], tempResult[3], tempResult[4], 
                                           tempResult[5], tempResult[6], tempResult[7], tempResult[8], tempResult[9], 
                                           tempResult[10], tempResult[11], tempResult[12], tempResult[13]));
                                           */
                i++;
            }


            reader.Close();

            return result;
        }

    }
}