using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.ProductComponents
{
    class HardDrive : ComputerComponents
    {
        bool isInternal;
        string type = "";
        string formFactor = "";
        string capacity = "";
        string cacheSize = "";
        string transferRate = "";
        string brand = "";
        string sata = "";
        string height = "";
        string depth = "";
        string width = "";


        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "intern/ ekstern":
                        isInternal = (info.Value.ToLower() == "intern") ? true : false;
                        break;
                    case "type":
                        type = info.Value;
                        break;
                    case "formfaktor":
                        formFactor = info.Value;
                        break;
                    case "harddisk størrelse":
                        capacity = info.Value;
                        break;
                    case "cachehukommelse":
                        cacheSize = info.Value;
                        break;
                    case "transfer rate":
                        transferRate = info.Value;
                        break;
                    case "mærke":
                        brand = Regex.Replace(info.Value, "(<.*?>)+", "");
                        break;
                    case "sata":
                        sata = info.Value;
                        break;
                    case "højde":
                        height = info.Value;
                        break;
                    case "dybde":
                        depth = info.Value;
                        break;
                    case "bredde":
                        width = info.Value;
                        break;
                }
            }
        }

        public bool IsInternal
        {
            get { return isInternal; }
        }

        public string Type
        {
            get { return type; }
        }

        public string FormFactor
        {
            get { return formFactor; }
        }

        public string Capacity
        {
            get { return capacity; }
        }

        public string CacheSize
        {
            get { return cacheSize; }
        }

        public string TransferRate
        {
            get { return transferRate; }
        }

        public string Brand
        {
            get { return brand; }
        }

        public string Sata
        {
            get { return sata; }
        }

        public string Height
        {
            get { return height; }
        }

        public string Depth
        {
            get { return depth; }
        }

        public string Width
        {
            get { return width; }
        }

        public override void InsertComponentToDB(int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO HardDrive" +
                                                    "(ProductID, isInternal, type, formFactor, capacity, cacheSize," +
                                                    " transforRate, brand, sata, height, depth, width)" +
                                                    "VALUES(@ProductID, @isInternal, @type, @formFactor, @capacity, " +
                                                    " @cacheSize, @transforRate, @brand, @sata, @height, @depth, @width)",
                connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@isInternal", IsInternal);
            command.Parameters.AddWithValue("@type", Type);
            command.Parameters.AddWithValue("@formFactor", FormFactor);
            command.Parameters.AddWithValue("@capacity", Capacity);
            command.Parameters.AddWithValue("@cacheSize", CacheSize);
            command.Parameters.AddWithValue("@transforRate", TransferRate);
            command.Parameters.AddWithValue("@brand", Brand);
            command.Parameters.AddWithValue("@sata", Sata);
            command.Parameters.AddWithValue("@height", Height);
            command.Parameters.AddWithValue("@depth", Depth);
            command.Parameters.AddWithValue("@width", Width);

            command.ExecuteNonQuery();
        }
    }
}