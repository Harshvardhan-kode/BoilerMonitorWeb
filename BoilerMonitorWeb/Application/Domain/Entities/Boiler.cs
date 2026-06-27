using System;

namespace BoilerMonitorWeb.Application.Domain.Entities
{
    public class Boiler
    {
        public int BoilerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal? Capacity_KGHR { get; set; }
        public bool IsActive { get; set; }
    }
}