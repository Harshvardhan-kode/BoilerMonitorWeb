using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BoilerMonitorWeb.Application.Domain.Entities;
using BoilerMonitorWeb.Application.Interfaces;
using BoilerMonitorWeb.Infrastructure.Data;

namespace BoilerMonitorWeb.Infrastructure.Services
{
    public class BoilerDataService : IBoilerService, ITelemetryService, IAlarmService
    {
        private readonly AppDbContext _context;

        public BoilerDataService(AppDbContext context)
        {
            _context = context;
        }

        // IBoilerService Implementation
        public async Task<IEnumerable<Boiler>> GetAllBoilersAsync()
        {
            return await _context.Boilers.AsNoTracking().ToListAsync();
        }

        // ITelemetryService Implementation: Grabs the absolute latest log entry
        public async Task<Telemetry?> GetLatestForBoilerAsync(int boilerId)
        {
            return await _context.BoilerLogs
                .Where(l => l.BoilerID == boilerId)
                .OrderByDescending(l => l.LogTimestamp)
                .FirstOrDefaultAsync();
        }

        // IAlarmService Implementation: Gets count of active (unacknowledged) alarms
        public async Task<int> GetActiveAlarmCountAsync(int boilerId)
        {
            return await _context.AlarmLogs
                .CountAsync(a => a.BoilerID == boilerId && !a.Acknowledged);
        }

        // IAlarmService Implementation: Grabs highest severity level among unacknowledged alarms
        public async Task<string> GetWorstActiveSeverityAsync(int boilerId)
        {
            var activeAlarms = await _context.AlarmLogs
                .Where(a => a.BoilerID == boilerId && !a.Acknowledged)
                .Select(a => a.AlarmDefID)
                .ToListAsync();

            if (!activeAlarms.Any()) return "Normal";

            // Cross-reference with definitions to see what severities are active
            var severities = await _context.AlarmDefinitions
                .Where(d => activeAlarms.Contains(d.AlarmDefID))
                .Select(d => d.Severity)
                .ToListAsync();

            if (severities.Contains("Danger")) return "Danger";
            if (severities.Contains("Critical")) return "Critical";
            if (severities.Contains("Warning")) return "Warning";

            return "Normal";
        }
    }
}