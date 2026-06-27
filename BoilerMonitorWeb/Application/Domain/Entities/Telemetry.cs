using System;

namespace BoilerMonitorWeb.Application.Domain.Entities
{
    public class Telemetry
    {
        public int BoilerID { get; set; }
        public DateTime LogTimestamp { get; set; }
        public decimal SteamPressure_Bar { get; set; }
        public decimal SteamTemp_C { get; set; }
        public decimal WaterLevel_mm { get; set; }
        public decimal FeedwaterTemp_C { get; set; }
        public decimal FlueGasTemp_C { get; set; }
        public decimal O2_Percentage { get; set; }
        public decimal SteamFlow_KGHR { get; set; }
    }
}