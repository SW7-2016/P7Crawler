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

        protected override void AddInformation(List<string[]> productInformation)
        {
            foreach (string[] info in productInformation)
            {
                switch (info[0].ToLower())
                {
                    case "processor serie":
                        cpuSeries = info[1];
                        break;
                    case "processor model":
                        model = info[1];
                        break;
                    case "fabrikant":
                        model = info[1];
                        break;
                    case "clockfrekvens":
                        clock = info[1];
                        break;
                    case "clock-frekvens":
                        clock = info[1];
                        break;
                    case "integreret gpu":
                        integratedGpu = info[1];
                        break;
                    case "type":
                        if (info[2] == "4" || info[2] == "3")
                        {
                            if (!info[1].Contains("garanti"))
                            {
                                integratedGpu = info[1];
                            }
                        }
                        if (info[2] == "2")
                        {
                            cpuSeries = info[1];
                        }
                        break;
                    case "boxed (inkluderer blæser eller køler)":
                        stockCooler = (info[1].ToLower() == "ja") ? true : false;
                        break;
                    case "mærke":
                        manufacturer = Regex.Replace(info[1], "(<.*?>)+", "");
                        break;
                    case "multi-core":
                    case "processorkerner":
                        Match noOfCores = Regex.Match(info[1], "\\d*");
                        physicalCores = int.Parse(noOfCores.Value);
                        break;
                    case "antal tråde":
                    case "processor tråde":
                        Match noOfThreads = Regex.Match(info[1], "\\d*");
                        logicalCores = int.Parse(noOfThreads.Value);
                        break;
                    case "max turbo frequency":
                        maxTurbo = info[1];
                        break;
                    case "max turbo speed":
                        maxTurbo = info[1];
                        break;
                    case "sokkel":
                        socket = info[1];
                        break;
                    case "kombatibel processor tilslutning":
                        socket = info[1];
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