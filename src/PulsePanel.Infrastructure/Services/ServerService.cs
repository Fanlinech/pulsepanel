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
            CheckPort = request.CheckPort,
            Description = request.Description,
            LastCheckAt = DateTime.UtcNow,
            LastCheckMessage = null,
            LastResponseTimeMs = 0,
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
            CheckPort = newServer.CheckPort,
            Description = newServer.Description,
            CreatedAt = newServer.CreatedAt,
            LastCheckAt = newServer.LastCheckAt,
            LastCheckMessage = newServer.LastCheckMessage,
            LastResponseTimeMs = newServer.LastResponseTimeMs,
            LastHeartbeatAt = newServer.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(newServer.LastHeartbeatAt,newServer.LastCheckAt, newServer.LastCheckMessage)
        };
        }

    public async Task<IReadOnlyList<ServerResponse>> GetAllAsync(GetServersRequest request)
    {
        var query = _dbContext.Servers
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(server =>
                server.Name.ToLower().Contains(search) ||
                server.Host.ToLower().Contains(search) ||
                (server.Description != null && server.Description.ToLower().Contains(search)));
        }

        var servers = await query.ToListAsync();

        var response = servers
            .Select(server => new ServerResponse
            {
                Id = server.Id,
                Name = server.Name,
                Host = server.Host,
                CheckPort = server.CheckPort,
                Description = server.Description,
                CreatedAt = server.CreatedAt,
                LastCheckAt = server.LastCheckAt,
                LastCheckMessage = server.LastCheckMessage,
                LastResponseTimeMs = server.LastResponseTimeMs,
                LastHeartbeatAt = server.LastHeartbeatAt,
                Status = _statusCalc.GetStatus(server.LastHeartbeatAt, server.LastCheckAt, server.LastCheckMessage)
            })
            .AsEnumerable();

        var sortBy = request.SortBy?.ToLower() ?? "createdat";
        var sortDirection = request.SortDirection?.ToLower() ?? "desc";

        response = sortBy switch
        {
            "name" => sortDirection == "asc"
                ? response.OrderBy(server => server.Name)
                : response.OrderByDescending(server => server.Name),

            "host" => sortDirection == "asc"
                ? response.OrderBy(server => server.Host)
                : response.OrderByDescending(server => server.Host),

            "status" => sortDirection == "asc"
                ? response.OrderBy(server => server.Status)
                : response.OrderByDescending(server => server.Status),

            "lastheartbeatat" => sortDirection == "asc"
                ? response.OrderBy(server => server.LastHeartbeatAt)
                : response.OrderByDescending(server => server.LastHeartbeatAt),

            "createdat" => sortDirection == "asc"
                ? response.OrderBy(server => server.CreatedAt)
                : response.OrderByDescending(server => server.CreatedAt),

            _ => response.OrderByDescending(server => server.CreatedAt)
        };

        return response.ToList();
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
            CheckPort = server.CheckPort,
            Description = server.Description,
            CreatedAt = server.CreatedAt,
            LastCheckAt = server.LastCheckAt,
            LastCheckMessage = server.LastCheckMessage,
            LastResponseTimeMs = server.LastResponseTimeMs,
            LastHeartbeatAt = server.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(server.LastHeartbeatAt, server.LastCheckAt, server.LastCheckMessage)
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
            CheckPort = server.CheckPort,
            Description = server.Description,
            CreatedAt = server.CreatedAt,
            LastCheckAt = server.LastCheckAt,
            LastCheckMessage = server.LastCheckMessage,
            LastResponseTimeMs = server.LastResponseTimeMs,
            LastHeartbeatAt = server.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(server.LastHeartbeatAt, server.LastCheckAt, server.LastCheckMessage)
        };
    }

    public async Task<ServerResponse?> UpdateAsync(Guid id, UpdateServerRequest request)
    {
        var server = await _dbContext.Servers.FindAsync(id);
        if (server is null) { return null; }
        server.Name = request.Name;
        server.Host = request.Host;
        server.CheckPort = request.CheckPort;
        server.Description = request.Description;
        await _dbContext.SaveChangesAsync();
        return new ServerResponse
        {
            Id = server.Id,
            Name = server.Name,
            Host = server.Host,
            CheckPort = server.CheckPort,
            Description = server.Description,
            CreatedAt = server.CreatedAt,
            LastCheckAt = server.LastCheckAt,
            LastCheckMessage = server.LastCheckMessage,
            LastResponseTimeMs = server.LastResponseTimeMs,
            LastHeartbeatAt = server.LastHeartbeatAt,
            Status = _statusCalc.GetStatus(server.LastHeartbeatAt, server.LastCheckAt, server.LastCheckMessage)
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