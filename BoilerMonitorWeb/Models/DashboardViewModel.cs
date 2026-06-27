using System.Collections.Generic;

namespace BoilerMonitorWeb.Models
{
    public class DashboardViewModel
    {
        public List<BoilerCardViewModel> Boilers { get; set; } = new();
        public int TotalBoilersOnline { get; set; }
        public int TotalActiveAlarms { get; set; }
        public decimal? AverageSystemPressure { get; set; }
    }
}