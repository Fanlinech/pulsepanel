using Microsoft.EntityFrameworkCore;
using PulsePanel.Core;
using PulsePanel.Core.DTOs.Dashboard;
using PulsePanel.Core.DTOs.Servers;
using PulsePanel.Core.Interfaces;
using PulsePanel.Core.Services;
using PulsePanel.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;



namespace PulsePanel.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _dbContext;
        private readonly ServerStatusCalculator _statusCalc;
        public DashboardService(AppDbContext dbContext, ServerStatusCalculator statusCalc)
        {
            _dbContext = dbContext;
            _statusCalc = statusCalc;
        }

        

        public async Task<DashboardSummaryResponse> GetSummaryAsync()
        {
            var summary = new DashboardSummaryResponse();
            var servers = await _dbContext.Servers
            .AsNoTracking()
            .Select(server => new ServerResponse
            {
                Id = server.Id,
                Name = server.Name,
                Host = server.Host,
                Description = server.Description,
                CreatedAt = server.CreatedAt,
                LastHeartbeatAt = server.LastHeartbeatAt,
                Status = _statusCalc.GetStatus(server.LastHeartbeatAt, server.LastCheckAt, server.LastCheckMessage)
            })
            .ToListAsync();

            foreach (var server in servers)
            {
                summary.TotalServers++;
                switch(server.Status)
                {
                    case ServerStatus.Online:
                        summary.OnlineServers++;
                        break;
                    case ServerStatus.Offline: 
                        summary.OfflineServers++;
                        break;
                    case ServerStatus.Unknown:
                        summary.UnknownServers++;
                        break;
                }
            }
            return summary;

        }
    }
}
