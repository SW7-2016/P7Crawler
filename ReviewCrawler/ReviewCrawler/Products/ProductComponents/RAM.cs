using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.ProductComponents
{
    class RAM : ComputerComponents
    {
        private string type = "";
        private string capacity = "";
        private string speed = "";
        private string technology = "";
        private string formFactor = "";
        private string casLatens = "";
        private string brand = "";


        protected override void AddInformation(List<string[]> productInformation)
        {
            foreach (string[] info in productInformation)
            {
                switch (info[0].ToLower())
                {
                    case "ram":
                        type = info[1];
                        break;
                    case "lagerkapacitet":
                        capacity = info[1];
                        break;
                    case "hukommelsesfrekvens":
                        speed = info[1];
                        break;
                    case "hukommelsesteknologi":
                        technology = info[1];
                        break;
                    case "model":
                        if (info[2] != "0")
                        {
                            formFactor = info[1];
                        }
                        break;
                    case "cas latency (rotering)":
                        casLatens = info[1];
                        break;
                    case "mærke": // OR "Fabrikant"
                        brand = info[1];
                        break;
                }
            }
        }


        public string Type
        {
            get
            {
                return type;
            }
        }

        public string Capacity
        {
            get
            {
                return capacity;
            }
        }

        public string Speed
        {
            get
            {
                return speed;
            }
        }

        public string Technology
        {
            get
            {
                return technology;
            }
        }

        public string FormFactor
        {
            get
            {
                return formFactor;
            }
        }

        public string CasLatens
        {
            get
            {
                return casLatens;
            }
        }

        public override void InsertComponentToDB(int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO RAM" +
                      "(ProductID, capacity, technology, formFactor, speed, casLatens, type)" +
                      "VALUES(@ProductID, @capacity, @technology, @formFactor, @speed, @casLatens, @type)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@capacity", Capacity);
            command.Parameters.AddWithValue("@technology", Technology);
            command.Parameters.AddWithValue("@formFactor", FormFactor);
            command.Parameters.AddWithValue("@speed", Speed);
            command.Parameters.AddWithValue("@casLatens", CasLatens);
            command.Parameters.AddWithValue("@type", Type);

            command.ExecuteNonQuery();

            //Testing purposes only
            MainWindow.pdRAM++;
            //
        }
    }
}
