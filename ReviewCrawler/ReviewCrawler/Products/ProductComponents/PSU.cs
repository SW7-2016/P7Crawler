using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.ProductComponents
{
    class PSU : ComputerComponents
    {
        string power = "";
        string formFactor = "";
        bool modular;
        string brand = "";
        string height = "";
        string depth = "";
        string width = "";
        string weight = "";

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "effekt":
                        power = info.Value;
                        break;
                    case "formfaktor":
                        formFactor = info.Value;
                        break;
                    case "modularitet":
                        modular = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "mærke":
                        brand = Regex.Replace(info.Value, "(<.*?>)+", "");
                        break;
                    case "vægt":
                        weight = info.Value;
                        break;
                    case "højde":
                        height = info.Value;
                        break;
                    case "bredde":
                        depth = info.Value;
                        break;
                    case "dybde":
                        width = info.Value;
                        break;
                }
            }
        }

        public string Power
        {
            get
            {
                return power;
            }
        }

        public string FormFactor
        {
            get
            {
                return formFactor;
            }
        }

        public bool Modular
        {
            get
            {
                return modular;
            }
        }

        public string Brand
        {
            get
            {
                return brand;
            }
        }

        public string Height
        {
            get
            {
                return height;
            }
        }

        public string Depth
        {
            get
            {
                return depth;
            }
        }

        public string Width
        {
            get
            {
                return width;
            }
        }

        public string Weight
        {
            get
            {
                return weight;
            }
        }

        public override void InsertComponentToDB(int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO PSU" +
                      "(ProductID, power, formFactor, modular, width, depth, height, weight, brand)" +
                      "VALUES(@ProductID, @power, @formFactor, @modular, @width, @depth, @height, @weight, @brand)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@power", Power);
            command.Parameters.AddWithValue("@formFactor", FormFactor);
            command.Parameters.AddWithValue("@modular", Modular);
            command.Parameters.AddWithValue("@width", Width);
            command.Parameters.AddWithValue("@depth", Depth);
            command.Parameters.AddWithValue("@height", Height);
            command.Parameters.AddWithValue("@weight", Weight);
            command.Parameters.AddWithValue("@brand", Brand);

            command.ExecuteNonQuery();
        }
    }
}
