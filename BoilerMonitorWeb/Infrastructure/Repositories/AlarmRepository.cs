using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BoilerMonitorWeb.Application.Domain.Entities;
using BoilerMonitorWeb.Application.Interfaces;
using BoilerMonitorWeb.Infrastructure.Data;

namespace BoilerMonitorWeb.Infrastructure.Repositories
{
    public class AlarmRepository : IAlarmRepository
    {
        private readonly AppDbContext _context;

        public AlarmRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAlarmAsync(AlarmLog alarm)
        {
            _context.AlarmLogs.Add(alarm);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AlarmDefinition>> GetDefinitionsAsync()
        {
            return await _context.AlarmDefinitions.AsNoTracking().ToListAsync();
        }
    }
}