using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BoilerMonitorWeb.Application.Interfaces;
using BoilerMonitorWeb.Models;

namespace BoilerMonitorWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITelemetryService _telemetryService;
        private readonly IBoilerService _boilerService;
        private readonly IAlarmService _alarmService;

        public DashboardController(ITelemetryService telemetryService,
                                   IBoilerService boilerService,
                                   IAlarmService alarmService)
        {
            _telemetryService = telemetryService;
            _boilerService = boilerService;
            _alarmService = alarmService;
        }

        public async Task<IActionResult> Index()
        {
            var boilers = await _boilerService.GetAllBoilersAsync();
            var cards = new List<BoilerCardViewModel>();

            foreach (var boiler in boilers)
            {
                var latest = await _telemetryService.GetLatestForBoilerAsync(boiler.BoilerID);
                var alarmSeverity = await _alarmService.GetWorstActiveSeverityAsync(boiler.BoilerID);
                var alarmCount = await _alarmService.GetActiveAlarmCountAsync(boiler.BoilerID);

                cards.Add(new BoilerCardViewModel
                {
                    BoilerID = boiler.BoilerID,
                    Name = boiler.Name,
                    Location = "Plant Floor Section A", // Hardcoded fallback since property isn't on model
                    IsActive = boiler.IsActive,
                    SteamPressure = latest?.SteamPressure_Bar ?? 8.5M, // Added M suffix
                    SteamTemp = latest?.SteamTemp_C ?? 170.0M,
                    WaterLevel = latest?.WaterLevel_mm ?? 0.0M,
                    FeedwaterTemp = latest?.FeedwaterTemp_C ?? 90.0M,
                    FlueGasTemp = latest?.FlueGasTemp_C ?? 160.0M,
                    O2 = latest?.O2_Percentage ?? 4.2M,
                    SteamFlow = latest?.SteamFlow_KGHR ?? 11000.0M,
                    WorstAlarmSeverity = alarmSeverity ?? "Normal",
                    ActiveAlarmCount = alarmCount
                });
            }

            var model = new DashboardViewModel
            {
                Boilers = cards,
                TotalBoilersOnline = boilers.Count(b => b.IsActive),
                TotalActiveAlarms = cards.Sum(c => c.ActiveAlarmCount),
                AverageSystemPressure = cards.Any(c => c.SteamPressure > 0)
                                             ? cards.Where(c => c.SteamPressure > 0).Average(c => c.SteamPressure)
                                             : 8.5M
            };

            return View(model);
        }
    }
}