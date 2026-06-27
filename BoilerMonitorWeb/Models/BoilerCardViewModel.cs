namespace BoilerMonitorWeb.Models
{
    public class BoilerCardViewModel
    {
        public int BoilerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsActive { get; set; }

        public decimal? SteamPressure { get; set; }
        public decimal? SteamTemp { get; set; }
        public decimal? WaterLevel { get; set; }
        public decimal? FeedwaterTemp { get; set; }
        public decimal? FlueGasTemp { get; set; }
        public decimal? O2 { get; set; }
        public decimal? SteamFlow { get; set; }

        public string WorstAlarmSeverity { get; set; } = "Normal";  // "Normal", "Warning", "Critical", "Danger"
        public int ActiveAlarmCount { get; set; }
    }
}