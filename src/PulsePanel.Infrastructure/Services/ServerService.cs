using Microsoft.EntityFrameworkCore;
using PulsePanel.Core;
using PulsePanel.Core.DTOs.Servers;
using PulsePanel.Core.Entities;
using PulsePanel.Core.Interfaces;
using PulsePanel.Core.Services;
using PulsePanel.Infrastructure.Persistence;
namespace PulsePanel.Infrastructure.Services;

public class ServerService : IServerService
{
    private readonly AppDbContext _dbContext;
    private readonly ServerStatusCalculator _statusCalc;

    public ServerService(AppDbContext dbContext, ServerStatusCalculator statusCalc)
    {
        _dbContext = dbContext;
        _statusCalc = statusCalc;
    }

    public async Task<ServerResponse> CreateAsync(CreateServerRequest request)
    {
        var newServer = new Server
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Host = request.Host,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            Status = ServerStatus.Unknown
        };

        _dbContext.Servers.Add(newServer);
        await _dbContext.SaveChangesAsync();

        return new ServerResponse
        {
            Id = newServer.Id,
            Name = newServer.Name,
            Host = newServer.Host,
            Description = newServer.Description,
            CreatedAt = newServer.CreatedAt,
            LastHeartbeatAt = newServer.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(newServer.LastHeartbeatAt)
        };
        }

    public async Task<IReadOnlyList<ServerResponse>> GetAllAsync()
    {
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
                Status = _statusCalc.GetStatus(server.LastHeartbeatAt)
            })
            .ToListAsync();

        return servers;
    }

    public async Task<ServerResponse?> GetByIDAsync(Guid id)
    {
        return await _dbContext.Servers
        .AsNoTracking()
        .Where(server => server.Id == id)
        .Select(server => new ServerResponse
        {
            Id = server.Id,
            Name = server.Name,
            Host = server.Host,
            Description = server.Description,
            CreatedAt = server.CreatedAt,
            LastHeartbeatAt = server.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(server.LastHeartbeatAt)
        })
        .FirstOrDefaultAsync();
    }

    public async Task<ServerResponse?> UpdateHeartbeatAsync(Guid id)
    {
        var server = await _dbContext.Servers.FindAsync(id);
        if (server is null) { return null; }
        server.LastHeartbeatAt = DateTime.UtcNow;
        server.Status = ServerStatus.Online;
        await _dbContext.SaveChangesAsync();
        return new ServerResponse
        {
            Id = server.Id,
            Name = server.Name,
            Host = server.Host,
            Description = server.Description,
            CreatedAt = server.CreatedAt,
            LastHeartbeatAt = server.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(server.LastHeartbeatAt)
        };
    }

    public async Task<ServerResponse?> UpdateAsync(Guid id, UpdateServerRequest request)
    {
        var server = await _dbContext.Servers.FindAsync(id);
        if (server is null) { return null; }
        server.Name = request.Name;
        server.Host = request.Host;
        server.Description = request.Description;
        await _dbContext.SaveChangesAsync();
        return new ServerResponse
        {
            Id = server.Id,
            Name = server.Name,
            Host = server.Host,
            Description = server.Description,
            CreatedAt = server.CreatedAt,
            LastHeartbeatAt = server.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(server.LastHeartbeatAt)
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var server = await _dbContext.Servers.FindAsync(id);
        if (server is null) { return false; }
        _dbContext.Remove(server);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}