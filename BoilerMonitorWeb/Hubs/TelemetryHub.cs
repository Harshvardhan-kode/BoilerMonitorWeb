using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BoilerMonitorWeb.Hubs
{
    public class TelemetryHub : Hub
    {
        // Clients can join specific groups if monitoring single boilers, 
        // but for a general overview, we'll broadcast directly.
        public async Task JoinDashboard()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "DashboardGroup");
        }
    }
}