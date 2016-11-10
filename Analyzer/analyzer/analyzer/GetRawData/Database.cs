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

        public void GetCpuData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM CPU", connection);
            command.Parameters.AddWithValue("@url", url);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)reader.GetValue(0);

            reader.Close();

            return result;
        }

    }
}