using System.Collections.Generic;
using System.Threading.Tasks;
using BoilerMonitorWeb.Application.Domain.Entities;

namespace BoilerMonitorWeb.Application.Interfaces
{
    public interface IBoilerService
    {
        Task<IEnumerable<Boiler>> GetAllBoilersAsync();
    }
}