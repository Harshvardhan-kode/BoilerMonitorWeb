using System;

namespace BoilerMonitorWeb.Application.Domain.Entities
{
    public class AlarmLog
    {
        public long AlarmID { get; set; }
        public int BoilerID { get; set; }
        public int AlarmDefID { get; set; }
        public DateTime OccurredAt { get; set; }
        public decimal CurrentValue { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Acknowledged { get; set; }
        public int? AckBy { get; set; }
        public DateTime? AckTimestamp { get; set; }
    }
}