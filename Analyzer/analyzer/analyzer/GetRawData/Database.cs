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
        

    }
}