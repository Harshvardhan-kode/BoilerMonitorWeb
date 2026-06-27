using System;
using System.Collections.Generic;

namespace BoilerMonitorWeb.Application.DTOs
{
    public class BatchValidationResultDto
    {
        public bool Passed { get; set; }
        public int TotalMinutes { get; set; }
        public int ViolationMinutes { get; set; }
        public int ToleranceMinutes { get; set; }
        public List<ViolationDetail> Violations { get; set; } = new();
        public string Summary { get; set; } = string.Empty;
    }

    public class ViolationDetail
    {
        public DateTime Timestamp { get; set; }
        public string Parameter { get; set; } = string.Empty;
        public decimal ActualValue { get; set; }
        public decimal? MinLimit { get; set; }
        public decimal? MaxLimit { get; set; }
    }
}