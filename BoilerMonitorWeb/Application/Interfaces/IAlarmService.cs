using System.Threading.Tasks;

namespace BoilerMonitorWeb.Application.Interfaces
{
    public interface IAlarmService
    {
        Task<string> GetWorstActiveSeverityAsync(int boilerId);
        Task<int> GetActiveAlarmCountAsync(int boilerId);
    }
}