using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoilerMonitorWeb.Application.Domain.Entities;
using BoilerMonitorWeb.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BoilerMonitorWeb.Application.Services
{
    public interface IAlarmEngine
    {
        Task<IReadOnlyList<AlarmLog>> EvaluateAsync(Telemetry telemetry);
    }

    public class AlarmEngine : IAlarmEngine
    {
        private readonly IAlarmRepository _alarmRepository;
        private readonly ILogger<AlarmEngine> _logger;
        private IReadOnlyList<AlarmDefinition>? _cachedDefinitions;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

        public AlarmEngine(IAlarmRepository alarmRepository, ILogger<AlarmEngine> logger)
        {
            _alarmRepository = alarmRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyList<AlarmLog>> EvaluateAsync(Telemetry telemetry)
        {
            var definitions = await GetDefinitionsAsync();
            var alarmsRaised = new List<AlarmLog>();

            foreach (var def in definitions)
            {
                decimal currentValue = GetParameterValue(telemetry, def.ParameterName);
                if (!IsValueWithinLimits(currentValue, def.MinValue, def.MaxValue))
                {
                    var alarm = new AlarmLog
                    {
                        BoilerID = telemetry.BoilerID,
                        AlarmDefID = def.AlarmDefID,
                        OccurredAt = telemetry.LogTimestamp,
                        CurrentValue = currentValue,
                        Message = BuildMessage(def.MessageTemplate, currentValue),
                        Acknowledged = false
                    };
                    alarmsRaised.Add(alarm);
                    _logger.LogWarning("Safety Breach Checked: {Message}", alarm.Message);
                }
            }

            return alarmsRaised;
        }

        private async Task<IReadOnlyList<AlarmDefinition>> GetDefinitionsAsync()
        {
            if (_cachedDefinitions is null)
            {
                await _cacheLock.WaitAsync();
                try
                {
                    if (_cachedDefinitions is null)
                    {
                        var defs = await _alarmRepository.GetDefinitionsAsync();
                        _cachedDefinitions = defs.ToList().AsReadOnly();
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
            }
            return _cachedDefinitions;
        }

        private static decimal GetParameterValue(Telemetry t, string parameterName)
        {
            return parameterName switch
            {
                "SteamPressure_Bar" => t.SteamPressure_Bar,
                "SteamTemp_C" => t.SteamTemp_C,
                "WaterLevel_mm" => t.WaterLevel_mm,
                "FeedwaterTemp_C" => t.FeedwaterTemp_C,
                "FlueGasTemp_C" => t.FlueGasTemp_C,
                "O2_Percentage" => t.O2_Percentage,
                "SteamFlow_KGHR" => t.SteamFlow_KGHR,
                _ => throw new ArgumentException($"Unknown sensor parameter mapping: {parameterName}")
            };
        }

        private static bool IsValueWithinLimits(decimal value, decimal? min, decimal? max)
        {
            if (min.HasValue && value < min.Value) return false;
            if (max.HasValue && value > max.Value) return false;
            return true;
        }

        private static string BuildMessage(string template, decimal value)
        {
            if (string.IsNullOrEmpty(template)) return $"Value {value:F2} breached operational boundary limits.";
            return template.Replace("{value}", value.ToString("F2"));
        }
    }
}