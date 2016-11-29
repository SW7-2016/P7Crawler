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
        string model = "";
        string series = "";
        bool modular;
        string brand = "";
        string height = "";
        string depth = "";
        string width = "";
        string weight = "";

        protected override void AddInformation(List<string[]> productInformation)
        {
            foreach (string[] info in productInformation)
            {
                switch (info[0].ToLower())
                {
                    case "effekt":
                        power = info[1];
                        break;
                    case "model":
                        model = info[1];
                        break;
                    case "produktlinje":
                        series = info[1];
                        break;
                    case "strømforsyning":
                        power = info[1];
                        break;
                    case "formfaktor":
                        formFactor = info[1];
                        break;
                    case "specifikationsoverensstemmelse":
                        formFactor = info[1];
                        break;
                    case "modularitet":
                        modular = (info[1].ToLower() == "ja") ? true : false;
                        break;
                    case "modulær kabel administration":
                        modular = (info[1].ToLower() == "ja") ? true : false;
                        break;
                    case "mærke":
                        brand = Regex.Replace(info[1], "(<.*?>)+", "");
                        break;
                    case "vægt":
                        weight = info[1];
                        break;
                    case "højde":
                        height = info[1];
                        break;
                    case "bredde":
                        depth = info[1];
                        break;
                    case "dybde":
                        width = info[1];
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

        public string Model
        {
            get
            {
                return model;
            }
        }

        public string Series
        {
            get
            {
                return series;
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

            //Testing purposes only
            MainWindow.pdPSU++;
            //
        }
    }
}
