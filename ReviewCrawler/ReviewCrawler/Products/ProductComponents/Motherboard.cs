using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ReviewCrawler.Products.ProductComponents
{
    class Motherboard : ComputerComponents
    {
        string formFactor = "";
        string chipset = "";
        bool netCard;
        bool soundCard;
        bool graphicsCard;
        bool multiGpu;
        bool crossfire;
        string cpuType = "";
        int cpuCount;
        string socket = "";
        bool sli;
        int maxMem;
        int memSlots;
        string memType = "";

        protected override void AddInformation(Dictionary<string, string> productInformation)
        {
            foreach (KeyValuePair<string, string> info in productInformation)
            {
                switch (info.Key.ToLower())
                {
                    case "formfaktor":
                        formFactor = info.Value;
                        break;
                    case "socket amd":
                        socket = info.Value;
                        break;
                    case "chipset amd":
                        chipset = info.Value;
                        break;
                    case "cpu support":
                        cpuCount = int.Parse(info.Value);
                        break;
                    case "sockets intel":
                        socket = info.Value;
                        break;
                    case "netværkskort indbygget":
                        netCard = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "grafikkort indbygget":
                        graphicsCard = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "multiple gpu support":
                        multiGpu = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "crossfire support":
                        crossfire = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "sli support":
                        sli = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "max ram mængde":
                        maxMem = int.Parse(info.Value);
                        break;
                    case "antal dimm-pladser":
                        memSlots = int.Parse(info.Value);
                        break;
                    case "ram type":
                        memType = info.Value;
                        break;
                    case "lydkort indbygget":
                        soundCard = (info.Value.ToLower() == "ja") ? true : false;
                        break;
                    case "chipset others":
                        chipset = info.Value;
                        break;
                    case "chipset intel":
                        chipset = info.Value;
                        break;
                    case "cpu type intel":
                        cpuType = info.Value;
                        break;
                }
            }
        }


        public string FormFactor
        {
            get { return formFactor; }
        }

        public string Chipset
        {
            get { return chipset; }
        }

        public bool NetCard
        {
            get { return netCard; }
        }

        public bool SoundCard
        {
            get { return soundCard; }
        }

        public bool GraphicsCard
        {
            get { return graphicsCard; }
        }

        public bool MultiGpu
        {
            get { return multiGpu; }
        }

        public bool Crossfire
        {
            get { return crossfire; }
        }

        public string CpuType
        {
            get { return cpuType; }
        }

        public int CpuCount
        {
            get { return cpuCount; }
        }

        public string Socket
        {
            get { return socket; }
        }

        public bool Sli
        {
            get { return sli; }
        }

        public int MaxMem
        {
            get { return maxMem; }
        }

        public int MemSlots
        {
            get { return memSlots; }
        }

        public string MemType
        {
            get { return memType; }
        }

        public override void InsertComponentToDB(int PID)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO Motherboard" +
                                                    "(ProductID, formFactor, cpuType, cpuCount, socket, netCard, soundCard," +
                                                    " multiGPU, crossfire, sli, maxMem, memSlots, memType, graphicsCard, chipset)" +
                                                    "VALUES(@ProductID, @formFactor, @cpuType, @cpuCount, @socket, @netCard, @soundCard," +
                                                    " @multiGPU, @crossFire, @sli, @maxMem, @memSlots, @memType, @graphicsCard, @chipset)",
                                                    connection);
            command.Parameters.AddWithValue("@ProductID", PID);
            command.Parameters.AddWithValue("@formFactor", FormFactor);
            command.Parameters.AddWithValue("@cpuType", CpuType);
            command.Parameters.AddWithValue("@cpuCount", CpuCount);
            command.Parameters.AddWithValue("@socket", Socket);
            command.Parameters.AddWithValue("@netCard", NetCard);
            command.Parameters.AddWithValue("@soundCard", SoundCard);
            command.Parameters.AddWithValue("@multiGPU", MultiGpu);
            command.Parameters.AddWithValue("@crossFire", Crossfire);
            command.Parameters.AddWithValue("@sli", Sli);
            command.Parameters.AddWithValue("@maxMem", MaxMem);
            command.Parameters.AddWithValue("@memSlots", MemSlots);
            command.Parameters.AddWithValue("@memType", MemType);
            command.Parameters.AddWithValue("@graphicsCard", GraphicsCard);
            command.Parameters.AddWithValue("@chipset", Chipset);

            command.ExecuteNonQuery();
        }
    }
}