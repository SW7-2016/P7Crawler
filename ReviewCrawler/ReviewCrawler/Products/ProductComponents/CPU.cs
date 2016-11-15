using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.ProductComponents
{
    class CPU : ComputerComponents
    {
        string model = "";
        string clock = "";
        string socket = "";
        bool stockCooler;
        string cpuSeries = "";
        int physicalCores;
        int logicalCores;
        string maxTurbo = "";
        string integratedGpu = "";
        string manufacturer = "";

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "processor serie":
                        cpuSeries = info.Value;
                        break;
                    case "processor model":
                        model = info.Value;
                        break;
                    case "clockfrekvens":
                        clock = info.Value;
                        break;
                    case "integreret gpu":
                        integratedGpu = info.Value;
                        break;
                    case "boxed (inkluderer blæser eller køler)":
                        stockCooler = (info.Value.ToLower() == "ja") ? true : false;
                        ;
                        break;
                    case "mærke":
                        manufacturer = Regex.Replace(info.Value, "(<.*?>)+", "");
                        break;
                    case "processorkerner":
                        Match noOfCores = Regex.Match(info.Value, "\\d*");
                        physicalCores = int.Parse(noOfCores.Value);
                        break;
                    case "processor tråde":
                        Match noOfThreads = Regex.Match(info.Value, "\\d*");
                        logicalCores = int.Parse(noOfThreads.Value);
                        break;
                    case "max turbo frequency":
                        maxTurbo = info.Value;
                        break;
                    case "sokkel":
                        socket = info.Value;
                        break;
                }
            }
        }

        public string Model
        {
            get { return model; }
        }

        public string Clock
        {
            get { return clock; }
        }

        public string Socket
        {
            get { return socket; }
        }

        public bool StockCooler
        {
            get { return stockCooler; }
        }

        public string CpuSeries
        {
            get { return cpuSeries; }
        }

        public int PhysicalCores
        {
            get { return physicalCores; }
        }

        public int LogicalCores
        {
            get { return logicalCores; }
        }

        public string MaxTurbo
        {
            get { return maxTurbo; }
        }

        public string IntegratedGpu
        {
            get { return integratedGpu; }
        }

        public string Manufacturer
        {
            get { return manufacturer; }
        }

        public override void InsertComponentToDB(int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO CPU" +
                                                    "(ProductID, model, clock, maxTurbo, integratedGpu," +
                                                    " stockCooler, manufacturer, cpuSeries, logicalCores, physicalCores, socket)" +
                                                    "VALUES(@ProductID, @model, @clock, @maxTurbo, @integratedGpu," +
                                                    " @stockcooler, @manufacturer, @cpuSeries, @logicalCores, @physicalCores, @socket)",
                connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@model", Model);
            command.Parameters.AddWithValue("@clock", Clock);
            command.Parameters.AddWithValue("@maxTurbo", MaxTurbo);
            command.Parameters.AddWithValue("@integratedGpu", IntegratedGpu);
            command.Parameters.AddWithValue("@stockCooler", StockCooler);
            command.Parameters.AddWithValue("@manufacturer", Manufacturer);
            command.Parameters.AddWithValue("@cpuSeries", CpuSeries);
            command.Parameters.AddWithValue("@logicalCores", LogicalCores);
            command.Parameters.AddWithValue("@physicalCores", PhysicalCores);
            command.Parameters.AddWithValue("@socket", socket);

            command.ExecuteNonQuery();
        }
    }
}