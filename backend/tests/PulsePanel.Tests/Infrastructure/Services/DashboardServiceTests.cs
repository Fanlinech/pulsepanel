using PulsePanel.Core;
using PulsePanel.Core.Entities;
using PulsePanel.Core.Services;
using PulsePanel.Infrastructure.Services;
using PulsePanel.Tests.Infrastructure;

namespace PulsePanel.Tests.Infrastructure.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_ReturnsZeros_WhenDatabaseIsEmpty()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var service = new DashboardService(dbContext, new ServerStatusCalculator());

        var summary = await service.GetSummaryAsync();

        Assert.Equal(0, summary.TotalServers);
        Assert.Equal(0, summary.OnlineServers);
        Assert.Equal(0, summary.OfflineServers);
        Assert.Equal(0, summary.UnknownServers);
    }

    [Fact]
    public async Task GetSummaryAsync_CountsOnlineAndOfflineServers()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var now = DateTime.UtcNow;

        dbContext.Servers.AddRange(
            CreateServer("Online heartbeat", lastHeartbeatAt: now.AddMinutes(-1)),
            CreateServer("Online check", lastCheckAt: now.AddMinutes(-1), lastCheckMessage: "Connection successful"),
            CreateServer("Offline heartbeat", lastHeartbeatAt: now.AddMinutes(-10)),
            CreateServer("Offline check", lastCheckAt: now.AddMinutes(-10), lastCheckMessage: "Connection successful"));
        await dbContext.SaveChangesAsync();

        var service = new DashboardService(dbContext, new ServerStatusCalculator());

        var summary = await service.GetSummaryAsync();

        Assert.Equal(4, summary.TotalServers);
        Assert.Equal(2, summary.OnlineServers);
        Assert.Equal(2, summary.OfflineServers);
        Assert.Equal(0, summary.UnknownServers);
    }

    private static Server CreateServer(
        string name,
        DateTime? lastHeartbeatAt = null,
        DateTime? lastCheckAt = null,
        string? lastCheckMessage = null)
    {
        return new Server
        {
            Id = Guid.NewGuid(),
            Name = name,
            Host = "127.0.0.1",
            CheckPort = 80,
            CreatedAt = DateTime.UtcNow,
            LastHeartbeatAt = lastHeartbeatAt,
            LastCheckAt = lastCheckAt ?? DateTime.MinValue,
            LastCheckMessage = lastCheckMessage,
            Status = ServerStatus.Unknown
        };
    }
}
