using System;
using System.Collections.Generic;
using BoilerMonitorWeb.Application.Domain.Entities;

namespace BoilerMonitorWeb.Models
{
    public class BoilerDetailViewModel
    {
        public Boiler Boiler { get; set; } = default!;
        public Telemetry? LatestTelemetry { get; set; }

        // Core Status Indicators from your services
        public string WorstAlarmSeverity { get; set; } = "None";
        public int ActiveAlarmsCount { get; set; }

        // Chart arrays
        public List<string> ChartLabels { get; set; } = new();
        public List<decimal> PressureData { get; set; } = new();
        public List<decimal> TempData { get; set; } = new();
        public List<decimal> WaterLevelData { get; set; } = new();
        public List<decimal> FeedwaterTempData { get; set; } = new();
        public List<decimal> FlueGasTempData { get; set; } = new();
        public List<decimal> O2Data { get; set; } = new();
        public List<decimal> FlowData { get; set; } = new();
    }
}