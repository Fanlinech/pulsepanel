using PulsePanel.Core.DTOs.Servers;
using PulsePanel.Core.Entities;
using PulsePanel.Core.Interfaces;
using PulsePanel.Core;
using PulsePanel.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace PulsePanel.Infrastructure.Services;

public class ServerService : IServerService
{
    private readonly AppDbContext _dbContext;

    public ServerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
            Status = newServer.Status
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
                Status = server.Status
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
            Status = server.Status
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
            Status = server.Status
        };
    }
}