using System.Collections.Generic;
using System.Threading.Tasks;
using BoilerMonitorWeb.Application.Domain.Entities;

namespace BoilerMonitorWeb.Application.Interfaces
{
    public interface IAlarmRepository
    {
        Task AddAlarmAsync(AlarmLog alarm);
        Task<IEnumerable<AlarmDefinition>> GetDefinitionsAsync();
    }
}