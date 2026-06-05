using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using PulsePanel.Core;
using PulsePanel.Core.Entities;
using PulsePanel.Core.Options;
using PulsePanel.Infrastructure.Persistence;
using PulsePanel.Infrastructure.Services;
using PulsePanel.Tests.Infrastructure;

namespace PulsePanel.Tests.Infrastructure.Services;

public class ServerCheckServiceTests
{
    [Fact]
    public async Task CheckAsync_ReturnsNull_WhenServerDoesNotExist()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var service = CreateService(dbContext);

        var result = await service.CheckAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CheckAsync_ReturnsOnlineAndUpdatesServer_WhenPortIsOpen()
    {
        using var listener = StartTcpListener(out var port);
        await using var dbContext = TestDbContextFactory.Create();
        var server = CreateServer(port);

        dbContext.Servers.Add(server);
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);

        var result = await service.CheckAsync(server.Id);
        var updatedServer = await dbContext.Servers.FindAsync(server.Id);

        Assert.NotNull(result);
        Assert.True(result.IsAvailable);
        Assert.Equal(ServerStatus.Online, result.Status);
        Assert.Equal("Connection successful", result.Message);
        Assert.Equal(ServerStatus.Online, updatedServer!.Status);
        Assert.Equal("Connection successful", updatedServer.LastCheckMessage);
        Assert.True(updatedServer.LastCheckAt > DateTime.MinValue);
    }

    [Fact]
    public async Task CheckAsync_ReturnsOfflineAndUpdatesServer_WhenPortIsClosed()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var server = CreateServer(GetClosedPort());

        dbContext.Servers.Add(server);
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);

        var result = await service.CheckAsync(server.Id);
        var updatedServer = await dbContext.Servers.FindAsync(server.Id);

        Assert.NotNull(result);
        Assert.False(result.IsAvailable);
        Assert.Equal(ServerStatus.Offline, result.Status);
        Assert.NotNull(result.Message);
        Assert.NotNull(updatedServer!.LastCheckMessage);

        var resultMessage = result.Message;
        var updatedMessage = updatedServer.LastCheckMessage;

        Assert.True(
            resultMessage == "Connection timeout" ||
            resultMessage.StartsWith("Connection error ->"));
        Assert.Equal(ServerStatus.Offline, updatedServer.Status);
        Assert.True(
            updatedMessage == "Connection timeout" ||
            updatedMessage.StartsWith("Connection error ->"));
        Assert.True(updatedServer.LastCheckAt > DateTime.MinValue);
    }

    [Fact]
    public async Task CheckAllAsync_ChecksAllServers()
    {
        using var listener = StartTcpListener(out var openPort);
        await using var dbContext = TestDbContextFactory.Create();
        var onlineServer = CreateServer(openPort);
        var offlineServer = CreateServer(GetClosedPort());

        dbContext.Servers.AddRange(onlineServer, offlineServer);
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);

        var results = await service.CheckAllAsync();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, result => result.ServerId == onlineServer.Id && result.Status == ServerStatus.Online);
        Assert.Contains(results, result => result.ServerId == offlineServer.Id && result.Status == ServerStatus.Offline);
    }

    private static ServerCheckService CreateService(AppDbContext dbContext)
    {
        var options = Options.Create(new ServerCheckOptions
        {
            TimeoutSeconds = 1,
            IntervalSeconds = 60
        });

        return new ServerCheckService(dbContext, options);
    }

    private static Server CreateServer(int port)
    {
        return new Server
        {
            Id = Guid.NewGuid(),
            Name = "Test server",
            Host = IPAddress.Loopback.ToString(),
            CheckPort = (ushort)port,
            CreatedAt = DateTime.UtcNow,
            LastCheckAt = DateTime.MinValue,
            Status = ServerStatus.Unknown
        };
    }

    private static TcpListener StartTcpListener(out int port)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        port = ((IPEndPoint)listener.LocalEndpoint).Port;

        return listener;
    }

    private static int GetClosedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();

        return port;
    }
}
