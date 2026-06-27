using System.Threading.Tasks;
using BoilerMonitorWeb.Application.Domain.Entities;
using BoilerMonitorWeb.Application.Interfaces;

namespace BoilerMonitorWeb.Application.Services
{
    public interface IAlarmManager
    {
        Task ProcessTelemetryAsync(Telemetry telemetry);
    }

    public class AlarmManager : IAlarmManager
    {
        private readonly IAlarmEngine _engine;
        private readonly IAlarmRepository _repository;

        public AlarmManager(IAlarmEngine engine, IAlarmRepository repository)
        {
            _engine = engine;
            _repository = repository;
        }

        public async Task ProcessTelemetryAsync(Telemetry telemetry)
        {
            var raisedAlarms = await _engine.EvaluateAsync(telemetry);
            foreach (var alarm in raisedAlarms)
            {
                await _repository.AddAlarmAsync(alarm);
            }
        }
    }
}