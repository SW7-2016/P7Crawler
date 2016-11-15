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


        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "ram":
                        type = info.Value;
                        break;
                    case "lagerkapacitet":
                        capacity = info.Value;
                        break;
                    case "hukommelsesfrekvens":
                        speed = info.Value;
                        break;
                    case "hukommelsesteknologi":
                        technology = info.Value;
                        break;
                    case "model":
                        formFactor = info.Value;
                        break;
                    case "cas latency (rotering)":
                        casLatens = info.Value;
                        break;
                    case "mærke":
                        brand = info.Value;
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
        }
    }
}
