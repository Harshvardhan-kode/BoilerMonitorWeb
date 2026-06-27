using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BoilerMonitorWeb.Application.Interfaces;
using BoilerMonitorWeb.Application.Domain.Entities;
using BoilerMonitorWeb.Models;

namespace BoilerMonitorWeb.Controllers
{
    [Authorize]
    public class BoilerController : Controller
    {
        private readonly IBoilerService _boilerService;
        private readonly ITelemetryService _telemetryService;
        private readonly IAlarmService _alarmService;

        public BoilerController(
            IBoilerService boilerService,
            ITelemetryService telemetryService,
            IAlarmService alarmService)
        {
            _boilerService = boilerService;
            _telemetryService = telemetryService;
            _alarmService = alarmService;
        }

        // GET: Boiler/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            // Find boiler from the available collection
            var boilers = await _boilerService.GetAllBoilersAsync() ?? new List<Boiler>();
            var boiler = boilers.FirstOrDefault(b => b.BoilerID == id);
            if (boiler == null) return NotFound();

            // Fetch live telemetry reading
            var latestTelemetry = await _telemetryService.GetLatestForBoilerAsync(id);

            // Fetch alarm summary data from your interfaces
            var worstSeverity = await _alarmService.GetWorstActiveSeverityAsync(id);
            var activeAlarmCount = await _alarmService.GetActiveAlarmCountAsync(id);

            // Generate clean mock/placeholder historical sequences so Chart.js doesn't crash
            var chartLabels = new List<string>();
            var pressureData = new List<decimal>();
            var tempData = new List<decimal>();
            var waterLevelData = new List<decimal>();
            var feedwaterTempData = new List<decimal>();
            var flueGasTempData = new List<decimal>();
            var o2Data = new List<decimal>();
            var flowData = new List<decimal>();

            // If online, seed some mock points for the last few hours for visualization stability
            if (latestTelemetry != null)
            {
                var baseTime = DateTime.Now.AddHours(-4);
                for (int i = 0; i <= 4; i++)
                {
                    chartLabels.Add(baseTime.AddHours(i).ToString("HH:mm"));
                    pressureData.Add(latestTelemetry.SteamPressure_Bar);
                    tempData.Add(latestTelemetry.SteamTemp_C);
                    waterLevelData.Add(latestTelemetry.WaterLevel_mm);
                    feedwaterTempData.Add(latestTelemetry.FeedwaterTemp_C);
                    flueGasTempData.Add(latestTelemetry.FlueGasTemp_C);
                    o2Data.Add(latestTelemetry.O2_Percentage);
                    flowData.Add(latestTelemetry.SteamFlow_KGHR);
                }
            }

            var model = new BoilerDetailViewModel
            {
                Boiler = boiler,
                LatestTelemetry = latestTelemetry,
                WorstAlarmSeverity = worstSeverity ?? "None",
                ActiveAlarmsCount = activeAlarmCount,
                ChartLabels = chartLabels,
                PressureData = pressureData,
                TempData = tempData,
                WaterLevelData = waterLevelData,
                FeedwaterTempData = feedwaterTempData,
                FlueGasTempData = flueGasTempData,
                O2Data = o2Data,
                FlowData = flowData
            };

            return View(model);
        }
    }
}