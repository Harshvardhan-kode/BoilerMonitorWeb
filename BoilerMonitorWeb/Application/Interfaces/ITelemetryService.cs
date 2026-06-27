using System.Threading.Tasks;
using BoilerMonitorWeb.Application.Domain.Entities;

namespace BoilerMonitorWeb.Application.Interfaces
{
    public interface ITelemetryService
    {
        Task<Telemetry?> GetLatestForBoilerAsync(int boilerId);
    }
}