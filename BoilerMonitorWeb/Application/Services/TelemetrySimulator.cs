using BoilerMonitorWeb.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BoilerMonitorWeb.Application.Services
{
    public class TelemetrySimulator : BackgroundService
    {
        private readonly IHubContext<TelemetryHub> _hubContext;
        private readonly Random _random = new();

        public TelemetrySimulator(IHubContext<TelemetryHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");

                // --- BOILER 1 (A1) SIMULATION ---
                var pressure1 = Math.Round(9.0 + Math.Sin(DateTime.UtcNow.Second / 5.0) * 0.6 + _random.NextDouble() * 0.3, 1);
                var temp1 = Math.Round(170.0 + (pressure1 * 0.4) + _random.NextDouble() * 1.5, 0);
                var flow1 = _random.Next(3100, 3350);
                var water1 = _random.Next(-2, 3);
                bool alarm1 = pressure1 > 9.8;

                await _hubContext.Clients.Group("DashboardGroup").SendAsync("ReceiveTelemetryUpdate", new
                {
                    timestamp = timestamp,
                    boilerId = 1,
                    pressure = pressure1,
                    temp = temp1,
                    flow = flow1,
                    waterLevel = water1,
                    activeAlarmsCount = alarm1 ? 1 : 0
                }, stoppingToken);

                // --- BOILER 2 (B2) SIMULATION ---
                var pressure2 = Math.Round(8.4 + _random.NextDouble() * 0.2, 1);
                var temp2 = _random.Next(173, 176);
                var flow2 = _random.Next(2850, 2950);
                var water2 = _random.Next(-1, 2);

                await _hubContext.Clients.Group("DashboardGroup").SendAsync("ReceiveTelemetryUpdate", new
                {
                    timestamp = timestamp,
                    boilerId = 2,
                    pressure = pressure2,
                    temp = temp2,
                    flow = flow2,
                    waterLevel = water2,
                    activeAlarmsCount = 0
                }, stoppingToken);

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}