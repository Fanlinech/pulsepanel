using PulsePanel.Core;
using PulsePanel.Core.DTOs.Servers;
using PulsePanel.Core.Interfaces;
using PulsePanel.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace PulsePanel.Infrastructure.Services
{
    public class ServerCheckService : IServerCheckService
    {
        private readonly AppDbContext _dbContext;

        public ServerCheckService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        

        public async Task<CheckServerResponse?> CheckAsync(Guid Id)
        {
            var server = await _dbContext.Servers.FindAsync(Id);
            if (server == null) { return null; }

            using var client = new TcpClient();

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var connectTask = client.ConnectAsync(server.Host, server.CheckPort);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3));
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                stopwatch.Stop();

                if (completedTask == timeoutTask)
                {
                    server.Status = ServerStatus.Offline;
                    server.LastCheckAt = DateTime.UtcNow;
                    server.LastCheckMessage = "Connection timeout";
                    server.LastResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                    await _dbContext.SaveChangesAsync();
                    return new CheckServerResponse
                    {
                        ServerId = server.Id,
                        Host = server.Host,
                        Port = server.CheckPort,
                        IsAvailable = false,
                        Status = server.Status,
                        CheckedAt = DateTime.UtcNow,
                        ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds,
                        Message = server.LastCheckMessage
                    };
                }

                await connectTask;

                server.Status = ServerStatus.Online;
                server.LastCheckAt = DateTime.UtcNow;
                server.LastCheckMessage = "Connection successful";
                server.LastResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                await _dbContext.SaveChangesAsync();
                return new CheckServerResponse
                {
                    ServerId = server.Id,
                    Host = server.Host,
                    Port = server.CheckPort,
                    IsAvailable = true,
                    Status = server.Status,
                    CheckedAt = DateTime.UtcNow,
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds,
                    Message = server.LastCheckMessage
                };
            }
            catch (SocketException ex)
            {
                stopwatch.Stop();
                server.Status = ServerStatus.Offline;
                server.LastCheckAt = DateTime.UtcNow;
                server.LastCheckMessage = $"Connection error -> {ex.Message}";
                server.LastResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                await _dbContext.SaveChangesAsync();
                return new CheckServerResponse
                {
                    ServerId = server.Id,
                    Host = server.Host,
                    Port = server.CheckPort,
                    IsAvailable = false,
                    Status = server.Status,
                    CheckedAt = DateTime.UtcNow,
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds,
                    Message = server.LastCheckMessage
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                server.Status = ServerStatus.Offline;
                server.LastCheckAt = DateTime.UtcNow;
                server.LastCheckMessage = $"Connection error -> {ex.Message}";
                server.LastResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                await _dbContext.SaveChangesAsync();
                return new CheckServerResponse
                {
                    ServerId = server.Id,
                    Host = server.Host,
                    Port = server.CheckPort,
                    IsAvailable = false,
                    Status = server.Status,
                    CheckedAt = DateTime.UtcNow,
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds,
                    Message = server.LastCheckMessage
                };
            }
        }
    }
}
