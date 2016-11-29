using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.ProductComponents
{
    class Chassis : ComputerComponents
    {
        string type = "";
        string series = "";
        string model = "";
        bool atx;
        bool miniAtx;
        bool miniItx;
        string fans = "";
        string brand = "";
        string weight = "";
        string height = "";
        string depth = "";
        string width = "";

        protected override void AddInformation(List<string[]> productInformation)
        {
            foreach (string[] info in productInformation)
            {
                switch (info[0].ToLower())
                {
                    case "type":
                        if (info[2] == "0")
                        {
                            type = info[1];
                        }
                        break;
                    case "kabinet type":
                        type = info[1];
                        break;
                    case "atx":
                        atx = (info[1].ToLower() == "ja") ? true : false;
                        break;
                    case "micro-atx":
                        miniAtx = (info[1].ToLower() == "ja") ? true : false;
                        break;
                    case "mini-itx":
                        miniItx = (info[1].ToLower() == "ja") ? true : false;
                        break;
                    case "blæsere":
                        fans = info[1];
                        break;
                    case "model":
                        model = info[1];
                        break;
                    case "produktlinje":
                        series = info[1];
                        break;
                    case "mærke":
                        if (info[2] == "0")
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
                    case "ventilator":
                        fans += "     " + info[1];
                        break;
                    case "understøttede motherboards":
                        if (info[1].ToLower().Contains("atx") && atx == false)
                        {
                            atx = true;
                            miniAtx = true;
                            miniItx = true;
                        }

                        if (info[1].ToLower().Contains("mini-atx") && atx == false)
                        {
                            miniAtx = true;
                        }

                        if (info[1].ToLower().Contains("mini-itx") && atx == false)
                        {
                            miniItx = true;
                        }
                        break;
                }
            }
        }

        public string Type
        {
            get { return type; }
        }

        public string Series
        {
            get { return series; }
        }

        public string Model
        {
            get { return model; }
        }

        public string Fans
        {
            get { return fans; }
        }

        public bool Atx
        {
            get { return atx; }
        }

        public bool MiniAtx
        {
            get { return miniAtx; }
        }

        public string Brand
        {
            get { return brand; }
        }

        public string Weight
        {
            get { return weight; }
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

        public bool MiniItx
        {
            get { return miniItx; }
        }

        public override void InsertComponentToDB(int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Chassis" +
                                                    "(ProductID,type,atx,miniAtx,miniItx,fans,brand,height,width,depth,weight)" +
                                                    "VALUES(@ProductID, @type, @atx, @miniAtx, @miniItx, @fans, @brand, @height, @width, @depth, @weight)",
                connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@type", Type);
            command.Parameters.AddWithValue("@atx", Atx);
            command.Parameters.AddWithValue("@miniAtx", MiniAtx);
            command.Parameters.AddWithValue("@miniItx", MiniItx);
            command.Parameters.AddWithValue("@fans", Fans);
            command.Parameters.AddWithValue("@brand", Brand);
            command.Parameters.AddWithValue("@height", Height);
            command.Parameters.AddWithValue("@width", Width);
            command.Parameters.AddWithValue("@depth", Depth);
            command.Parameters.AddWithValue("@weight", Weight);

            command.ExecuteNonQuery();
        }
    }
}