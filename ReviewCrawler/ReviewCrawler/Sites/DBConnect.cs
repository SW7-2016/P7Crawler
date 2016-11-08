using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using ReviewCrawler.Products.Reviews;
using ReviewCrawler.Products.Retailers;
using ReviewCrawler.Products.ProductComponents;
using ReviewCrawler.Products;
using System.Threading;
using System.Diagnostics;

namespace ReviewCrawler.Sites
{
    class DBConnect
    {
        private string connectionString = "server=172.25.23.57;database=crawlerdb;user=crawler;port=3306;password=Crawler23!;";
        public MySqlConnection connection;

        public void DbInitialize()
        {

            connection = new MySqlConnection(connectionString);

        }


        public void InsertReview(Review review)
        {
            if (!DoesReviewExist(review))
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO Review" +
                          "(reviewDate, crawlDate, content,productRating,reviewRating,author,positiveCount,negativeCount,verifiedPurchase,isCriticReview,productType,url,title,maxRating)"
                          + "VALUES(@reviewDate, @crawlDate, @content, @productRating, @reviewRating, @author, @positiveCount,"
                          + "@negativeCount, @verifiedPurchase, @isCriticReview, @productType, @url, @title, @maxRating)", connection);
                command.Parameters.AddWithValue("@reviewDate", DateToString(review.reviewDate));
                command.Parameters.AddWithValue("@crawlDate", DateToString(review.crawlDate));
                command.Parameters.AddWithValue("@content", review.content);
                command.Parameters.AddWithValue("@productRating", review.productRating);
                command.Parameters.AddWithValue("@reviewRating", review.reviewRating);
                command.Parameters.AddWithValue("@author", review.author);
                command.Parameters.AddWithValue("@positiveCount", review.reception.positive);
                command.Parameters.AddWithValue("@negativeCount", review.reception.negative);
                command.Parameters.AddWithValue("@verifiedPurchase", review.verifiedPurchase);
                command.Parameters.AddWithValue("@isCriticReview", review.isCriticReview);
                command.Parameters.AddWithValue("@productType", review.productType);
                command.Parameters.AddWithValue("@url", review.url);
                command.Parameters.AddWithValue("@title", review.title);
                command.Parameters.AddWithValue("@maxRating", review.maxRating);

                command.ExecuteNonQuery();

                int ID = GetReviewID(review.url);

                foreach (ReviewComment comment in review.comments)
                {
                    InsertReviewComment(comment, ID);
                }
            }
            else
            {
                Debug.WriteLine("Review " + review.url + " does already exist");
            }
        }

        public bool DoesReviewExist(Review review)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE url=@url", connection);
            command.Parameters.AddWithValue("@url", review.url);

            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private string DateToString(DateTime date)
        {
            return date.Year + "-" + date.Month + "-" + date.Day;
        }

        private void InsertReviewComment(ReviewComment comment, int ID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO ReviewComment" +
                      "(ReviewID, content, rating)" +
                      "VALUES(@ReviewID, @content, @rating)", connection);
            command.Parameters.AddWithValue("@ReviewID", ID);
            command.Parameters.AddWithValue("@content", comment.content);
            command.Parameters.AddWithValue("@rating", comment.rating);
            command.ExecuteNonQuery();
            
        }

        private int GetReviewID(string url)
        {
            MySqlCommand command = new MySqlCommand("SELECT ReviewID FROM Review WHERE url=@url", connection);
            command.Parameters.AddWithValue("@url", url);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)reader.GetValue(0);

            reader.Close();

            return result;
        }


        public bool DoesProductExist(Product product)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Product WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", product.name);

            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public void InsertProduct(Product product)
        {

            if (!DoesProductExist(product))
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO Product" +
              "(description, name)" +
              "VALUES(@description, @name)", connection);
                command.Parameters.AddWithValue("@description", product.description);
                command.Parameters.AddWithValue("@name", product.name);
                command.ExecuteNonQuery();

                int RID;

                int PID = GetProductID(product.name);


                InsertComponent(product, PID);


                foreach (var retailer in product.retailers)
                {
                    if (!RetailerExists(retailer.name))
                    {
                        InsertRetailer(retailer.name);
                    }

                    RID = GetRetailerID(retailer.name);

                    InsertProductRetailer(retailer, RID, PID);
                }
            }
            else
            {
                Debug.WriteLine("product " + product.name + " does already exist");
            }
        }

        private void InsertComponent(Product product, int PID)
        {

            if (product.GetType() == typeof(SoundCard))
            {
                InsertSoundCard((SoundCard)product, PID);
            }
            else if(product.GetType() == typeof(Chassis))
            {
                InsertChassis((Chassis)product, PID);
            }
            else if (product.GetType() == typeof(Cooling))
            {
                InsertCooling((Cooling)product, PID);
            }
            else if (product.GetType() == typeof(CPU))
            {
                InsertCPU((CPU)product, PID);
            }
            else if (product.GetType() == typeof(GPU))
            {
                InsertGPU((GPU)product, PID);
            }
            else if (product.GetType() == typeof(HardDrive))
            {
                InsertHardDrive((HardDrive)product, PID);
            }
            else if (product.GetType() == typeof(Motherboard))
            {
                InsertMotherboard((Motherboard)product, PID);
            }
            else if (product.GetType() == typeof(PSU))
            {
                InsertPSU((PSU)product, PID);
            }
            else if (product.GetType() == typeof(RAM))
            {
                InsertRAM((RAM)product, PID);
            }

            
        }

        private void InsertProductRetailer(Retailer retailer, int RID, int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Product_Retailer" +
          "(productID, retailerID, url, price)" +
          "VALUES(@productID, @retailerID, @url, @price)", connection);
            command.Parameters.AddWithValue("@productID", PID);
            command.Parameters.AddWithValue("@retailerID", RID);
            command.Parameters.AddWithValue("@url", retailer.url);
            command.Parameters.AddWithValue("@price", retailer.price);

            command.ExecuteNonQuery();
        }

        private void InsertRetailer(string retailer)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Retailer" +
                      "(name)" +
                      "VALUES(@name)", connection);
            command.Parameters.AddWithValue("@name", retailer);

            command.ExecuteNonQuery();
        }

        private int GetProductID(string name)
        {
            MySqlCommand command = new MySqlCommand("SELECT ProductID FROM Product WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", name);

            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            int result = (int)(reader.GetValue(0));

            reader.Close();

            return result;
        }

        private int GetRetailerID(string retailer)
        {
            MySqlCommand command = new MySqlCommand("SELECT RetailerID FROM Retailer WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", retailer);
            int result = 0;
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                result = (int)(reader.GetValue(0));
            }
            

            reader.Close();

            return result;
        }

        private bool RetailerExists(string retailer)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Retailer WHERE name=@name", connection);
            command.Parameters.AddWithValue("@name", retailer);

            if (command.ExecuteScalar() == null)
            {
                return false;
            }
            else
            {
                return true;
            }


        }

        private void InsertSoundCard(SoundCard soundCard, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO SoundCard" +
                      "(ProductID,type,speakerSupport,socket,fullDuplex)" +
                      "VALUES(@ProductID, @type, @speakerSupport, @socket, @fullDuplex)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@type", soundCard.Type);
            command.Parameters.AddWithValue("@speakerSupport", soundCard.SpeakerSupport);
            command.Parameters.AddWithValue("@socket", soundCard.Socket);
            command.Parameters.AddWithValue("@fullDuplex", soundCard.FullDuplex);
            command.ExecuteNonQuery();
        }

        private void InsertChassis(Chassis chassis, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO Chassis" +
                      "(ProductID,type,atx,miniAtx,miniItx,fans,brand,height,width,depth,weight)" +
                      "VALUES(@ProductID, @type, @atx, @miniAtx, @miniItx, @fans, @brand, @height, @width, @depth, @weight)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@type", chassis.Type);
            command.Parameters.AddWithValue("@atx", chassis.Atx);
            command.Parameters.AddWithValue("@miniAtx", chassis.MiniAtx);
            command.Parameters.AddWithValue("@miniItx", chassis.MiniItx);
            command.Parameters.AddWithValue("@fans", chassis.Fans);
            command.Parameters.AddWithValue("@brand", chassis.Brand);
            command.Parameters.AddWithValue("@height", chassis.Height);
            command.Parameters.AddWithValue("@width", chassis.Width);
            command.Parameters.AddWithValue("@depth", chassis.Depth);
            command.Parameters.AddWithValue("@weight", chassis.Weight);

            command.ExecuteNonQuery();
        }

        private void InsertCooling(Cooling cooling, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO Cooling" +
                      "(ProductID, type, speed, size, airflow, noise, connector)" +
                      "VALUES(@ProductID, @type, @speed, @size, @airflow, @noise, @connector)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@type", cooling.Type);
            command.Parameters.AddWithValue("@speed", cooling.Speed);
            command.Parameters.AddWithValue("@size", cooling.Size);
            command.Parameters.AddWithValue("@airflow", cooling.Airflow);
            command.Parameters.AddWithValue("@noise", cooling.Noise);
            command.Parameters.AddWithValue("@connector", cooling.Connector);

            command.ExecuteNonQuery();
        }



        private void InsertCPU(CPU cpu, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO CPU" +
                      "(ProductID, model, clock, maxTurbo, integratedGpu, stockCooler, manufacturer, cpuSeries, logicalCores, physicalCores)" +
                      "VALUES(@ProductID, @model, @clock, @maxTurbo, @integratedGpu, @stockcooler, @manufacturer, @cpuSeries, @logicalCores, @physicalCores)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@model", cpu.Model);
            command.Parameters.AddWithValue("@clock", cpu.Clock);
            command.Parameters.AddWithValue("@maxTurbo", cpu.MaxTurbo);
            command.Parameters.AddWithValue("@integratedGpu", cpu.IntegratedGpu);
            command.Parameters.AddWithValue("@stockCooler", cpu.StockCooler);
            command.Parameters.AddWithValue("@manufacturer", cpu.Manufacturer );
            command.Parameters.AddWithValue("@cpuSeries", cpu.CpuSeries);
            command.Parameters.AddWithValue("@logicalCores", cpu.LogicalCores);
            command.Parameters.AddWithValue("@physicalCores", cpu.PhysicalCores);

            command.ExecuteNonQuery();
        }

        private void InsertGPU(GPU gpu, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO GPU" +
                      "(ProductID, processorManufacturer, chipset, model, architecture, cooling, memSize, pciSlots, manufacturer)" +
                      "VALUES(@ProductID, @processorManufacturer, @chipset, @model, @architecture, @cooling, @memSize, @pciSlots, @manufacturer)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@processorManufacturer", gpu.ProcessorManufacturer);
            command.Parameters.AddWithValue("@chipset", gpu.Chipset);
            command.Parameters.AddWithValue("@model", gpu.Model);
            command.Parameters.AddWithValue("@architecture", gpu.Architecture);
            command.Parameters.AddWithValue("@cooling", gpu.Cooling);
            command.Parameters.AddWithValue("@memSize", gpu.MemSize);
            command.Parameters.AddWithValue("@pciSlots", gpu.PciSlots);
            command.Parameters.AddWithValue("@manufacturer", gpu.Manufacturer);

            command.ExecuteNonQuery();
        }

        private void InsertHardDrive(HardDrive hardDrive, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO HardDrive" +
                      "(ProductID, isInternal, type, formFactor, capacity, cacheSize, transforRate, brand, sata, height, depth, width)" +
                      "VALUES(@ProductID, @isInternal, @type, @formFactor, @capacity, @cacheSize, @transforRate, @brand, @sata, @height, @depth, @width)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@isInternal", hardDrive.IsInternal);
            command.Parameters.AddWithValue("@type", hardDrive.Type);
            command.Parameters.AddWithValue("@formFactor", hardDrive.FormFactor);
            command.Parameters.AddWithValue("@capacity", hardDrive.Capacity);
            command.Parameters.AddWithValue("@cacheSize", hardDrive.CacheSize);
            command.Parameters.AddWithValue("@transforRate", hardDrive.TransferRate);
            command.Parameters.AddWithValue("@brand", hardDrive.Brand);
            command.Parameters.AddWithValue("@sata", hardDrive.Sata);
            command.Parameters.AddWithValue("@height", hardDrive.Height);
            command.Parameters.AddWithValue("@depth", hardDrive.Depth);
            command.Parameters.AddWithValue("@width", hardDrive.Width);

            command.ExecuteNonQuery();
        }

        private void InsertMotherboard(Motherboard motherboard, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO Motherboard" +
                      "(ProductID, formFactor, cpuType, cpuCount, socket, netCard, soundCard, multiGPU, crossfire, sli, maxMem, memSlots, memType, graphicsCard, chipset)" +
                      "VALUES(@ProductID, @formFactor, @cpuType, @cpuCount, @socket, @netCard, @soundCard, @multiGPU, @crossFire, @sli, @maxMem, @memSlots, @memType, @graphicsCard, @chipset)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@formFactor", motherboard.FormFactor);
            command.Parameters.AddWithValue("@cpuType", motherboard.CpuType);
            command.Parameters.AddWithValue("@cpuCount", motherboard.CpuCount);
            command.Parameters.AddWithValue("@socket", motherboard.Socket);
            command.Parameters.AddWithValue("@netCard", motherboard.NetCard);
            command.Parameters.AddWithValue("@soundCard", motherboard.SoundCard);
            command.Parameters.AddWithValue("@multiGPU", motherboard.MultiGpu);
            command.Parameters.AddWithValue("@crossFire", motherboard.Crossfire);
            command.Parameters.AddWithValue("@sli", motherboard.Sli);
            command.Parameters.AddWithValue("@maxMem", motherboard.MaxMem);
            command.Parameters.AddWithValue("@memSlots", motherboard.MemSlots);
            command.Parameters.AddWithValue("@memType", motherboard.MemType);
            command.Parameters.AddWithValue("@graphicsCard", motherboard.GraphicsCard);
            command.Parameters.AddWithValue("@chipset", motherboard.Chipset);

            command.ExecuteNonQuery();
        }

        private void InsertPSU(PSU psu, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO PSU" +
                      "(ProductID, power, formFactor, modular, width, depth, height, weight, brand)" +
                      "VALUES(@ProductID, @power, @formFactor, @modular, @width, @depth, @height, @weight, @brand)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@power", psu.Power);
            command.Parameters.AddWithValue("@formFactor", psu.FormFactor);
            command.Parameters.AddWithValue("@modular", psu.Modular);
            command.Parameters.AddWithValue("@width", psu.Width);
            command.Parameters.AddWithValue("@depth", psu.Depth);
            command.Parameters.AddWithValue("@height", psu.Height);
            command.Parameters.AddWithValue("@weight", psu.Weight);
            command.Parameters.AddWithValue("@brand", psu.Brand);

            command.ExecuteNonQuery();
        }

        private void InsertRAM(RAM ram, int PID)
        {

            MySqlCommand command = new MySqlCommand("INSERT INTO RAM" +
                      "(ProductID, capacity, technology, formFactor, speed, casLatens, type)" +
                      "VALUES(@ProductID, @capacity, @technology, @formFactor, @speed, @casLatens, @type)", connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@capacity", ram.Capacity);
            command.Parameters.AddWithValue("@technology", ram.Technology);
            command.Parameters.AddWithValue("@formFactor", ram.FormFactor);
            command.Parameters.AddWithValue("@speed", ram.Speed);
            command.Parameters.AddWithValue("@casLatens", ram.CasLatens);
            command.Parameters.AddWithValue("@type", ram.Type);

            command.ExecuteNonQuery();
        }


    }
}
