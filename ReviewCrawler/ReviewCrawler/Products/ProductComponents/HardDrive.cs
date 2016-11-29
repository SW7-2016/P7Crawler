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
        string model = "";
        string series = "";
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


        protected override void AddInformation(List<string[]> productInformation)
        {
            foreach (string[] info in productInformation)
            {
                switch (info[0].ToLower())
                {
                    case "intern/ ekstern":
                        isInternal = (info[1].ToLower().Contains("intern")) ? true : false;
                        break;
                    case "harddisktype":
                        isInternal = (info[1].ToLower().Contains("intern")) ? true : false;
                        break;
                    case "type":
                        if (info[1].ToLower().Contains("harddisk") || info[1].ToLower().Contains("solid state drive"))
                        {
                            type = info[1];
                        }
                        break;
                    case "harddisk teknologi":
                        type = info[1];
                        break;
                    case "formfaktor":
                        formFactor = info[1];
                        break;
                    case "model":
                        model = info[1];
                        break;
                    case "produktlinje":
                        series = info[1];
                        break;
                    case "form factor (kort)":
                        formFactor = info[1];
                        break;
                    case "harddisk størrelse":
                        capacity = info[1];
                        break;
                    case "hd kapacitet":
                        capacity = info[1];
                        break;
                    case "cachehukommelse":
                        cacheSize = info[1];
                        break;
                    case "buffer størrelse":
                        cacheSize = info[1];
                        break;
                    case "transfer rate":
                        transferRate = info[1];
                        break;
                    case "overførselshastighed":
                        transferRate = info[1];
                        break;
                    case "internal data rate":
                        transferRate = info[1];
                        break;
                    case "internal data rate (skriv)":
                        transferRate = info[1];
                        break;
                    case "mærke":
                        brand = Regex.Replace(info[1], "(<.*?>)+", "");
                        break;
                    case "sata":
                        sata = info[1];
                        break;
                    case "lagrings-interface":
                        sata = info[1];
                        break;
                    case "grænseflade":
                        sata = info[1];
                        break;
                    case "højde":
                        height = info[1];
                        break;
                    case "dybde":
                        depth = info[1];
                        break;
                    case "bredde":
                        width = info[1];
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

        public string Model
        {
            get { return model; }
        }

        public string Series
        {
            get { return series; }
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
                                                    " transferRate, brand, sata, height, depth, width)" +
                                                    "VALUES(@ProductID, @isInternal, @type, @formFactor, @capacity, " +
                                                    " @cacheSize, @transferRate, @brand, @sata, @height, @depth, @width)",
                connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@isInternal", IsInternal);
            command.Parameters.AddWithValue("@type", Type);
            command.Parameters.AddWithValue("@formFactor", FormFactor);
            command.Parameters.AddWithValue("@capacity", Capacity);
            command.Parameters.AddWithValue("@cacheSize", CacheSize);
            command.Parameters.AddWithValue("@transferRate", TransferRate);
            command.Parameters.AddWithValue("@brand", Brand);
            command.Parameters.AddWithValue("@sata", Sata);
            command.Parameters.AddWithValue("@height", Height);
            command.Parameters.AddWithValue("@depth", Depth);
            command.Parameters.AddWithValue("@width", Width);

            command.ExecuteNonQuery();
        }
    }
}