using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.ProductComponents
{
    class GPU : ComputerComponents
    {
        string processorManufacturer = "";
        string chipset = "";
        string model = "";
        string graphicsProcessor = "";
        string architecture = "";
        int pciSlots;
        string cooling = "";
        string memSize = "";
        string manufacturer = "";
        string clockBoosted = "";
        string clock = "";


        protected override void AddInformation(List<string[]> productInformation)
        {
            foreach (string[] info in productInformation)
            {
                switch (info[0].ToLower())
                {
                    case "processor producent":
                        processorManufacturer = info[1];
                        break;
                    case "grafikprocessor producent":
                        processorManufacturer = info[1];
                        break;
                    case "model":
                        if (info[2] == "0")
                        {
                            model = info[1];
                        }
                        break;
                    case "mærke":
                        if (info[2] == "0")
                        {
                            manufacturer = info[1];
                        }
                        break;
                    case "geforce serie":
                        graphicsProcessor = info[1];
                        break;
                    case "grafikprocessor":
                        graphicsProcessor = info[1];
                        break;
                    case "chipset":
                        chipset = info[1];
                        break;
                    case "gpu architecture":
                        architecture = info[1];
                        break;
                    case "kortpladser":
                        pciSlots = int.Parse(info[1]);
                        break;
                    case "køling":
                        cooling = info[1];
                        break;
                    case "video hukommelse":
                        memSize = info[1];
                        break;
                    case "hukommelsesstørrelse":
                        memSize = info[1];
                        break;
                    case "radeon serie":
                        graphicsProcessor = info[1];
                        break;
                    case "clock":
                        clock = info[1];
                        break;
                    case "boost clockhastighed":
                        clockBoosted = info[1];
                        break;
                }
            }
        }

        public string ProcessorManufacturer
        {
            get { return processorManufacturer; }
        }

        public string Chipset
        {
            get { return chipset; }
        }

        public string Model
        {
            get { return model; }
        }

        public string Architecture
        {
            get { return architecture; }
        }

        public int PciSlots
        {
            get { return pciSlots; }
        }

        public string Cooling
        {
            get { return cooling; }
        }

        public string MemSize
        {
            get { return memSize; }
        }

        public string Manufacturer
        {
            get { return manufacturer; }
        }

        public override void InsertComponentToDB(int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO GPU" +
                                                    "(ProductID, processorManufacturer, chipset, graphicsProcessor," +
                                                    " architecture, cooling, memSize, pciSlots, manufacturer, clock, boostedClock, model)" +
                                                    "VALUES(@ProductID, @processorManufacturer, @chipset, @graphicsProcessor, " +
                                                    " @architecture, @cooling, @memSize, @pciSlots, @manufacturer, @clock, @boostedClock, @model)",
                connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@processorManufacturer", ProcessorManufacturer);
            command.Parameters.AddWithValue("@chipset", Chipset);
            command.Parameters.AddWithValue("@graphicsProcessor", graphicsProcessor);
            command.Parameters.AddWithValue("@architecture", Architecture);
            command.Parameters.AddWithValue("@cooling", Cooling);
            command.Parameters.AddWithValue("@memSize", MemSize);
            command.Parameters.AddWithValue("@pciSlots", PciSlots);
            command.Parameters.AddWithValue("@manufacturer", Manufacturer);
            command.Parameters.AddWithValue("@clock", clock);
            command.Parameters.AddWithValue("@boostedClock", clockBoosted);
            command.Parameters.AddWithValue("@model", Model);

            command.ExecuteNonQuery();
        }
    }
}