using Microsoft.EntityFrameworkCore;
using PulsePanel.Core;
using PulsePanel.Core.DTOs.Dashboard;
using PulsePanel.Core.DTOs.Servers;
using PulsePanel.Core.Interfaces;
using PulsePanel.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;



namespace PulsePanel.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _dbContext;
        public DashboardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static ServerStatus GetStatus(DateTime? LastHeartbeatAt)
        {
            if (LastHeartbeatAt is null)
            {
                return ServerStatus.Unknown;
            }

            return LastHeartbeatAt >= DateTime.UtcNow.AddMinutes(-5)
                ? ServerStatus.Online
                : ServerStatus.Offline;
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
                Status = GetStatus(server.LastHeartbeatAt)
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
