namespace BoilerMonitorWeb.Application.Domain.Entities
{
    public class AlarmDefinition
    {
        public int AlarmDefID { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string MessageTemplate { get; set; } = string.Empty;
    }
}