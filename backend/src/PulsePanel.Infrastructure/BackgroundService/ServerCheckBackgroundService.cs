using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulsePanel.Core.Interfaces;
using PulsePanel.Core.Options;

namespace PulsePanel.Infrastructure.BackgroundServices;

public class ServerCheckBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ServerCheckBackgroundService> _logger;
    private readonly IOptionsMonitor<ServerCheckOptions> _optionsMonitor;

    public ServerCheckBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ServerCheckBackgroundService> logger,
        IOptionsMonitor<ServerCheckOptions> optionsMonitor)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _optionsMonitor = optionsMonitor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Server check background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var options = _optionsMonitor.CurrentValue;

            if (!options.Enabled)
            {
                _logger.LogInformation("Automatic server checks are disabled");

                await Task.Delay(
                    TimeSpan.FromSeconds(options.IntervalSeconds),
                    stoppingToken);

                continue;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();

                var serverCheckService = scope.ServiceProvider
                    .GetRequiredService<IServerCheckService>();

                _logger.LogInformation("Starting automatic server checks");

                var results = await serverCheckService.CheckAllAsync();

                _logger.LogInformation(
                    "Automatic server checks completed. Checked servers: {Count}",
                    results.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Automatic server check cycle failed");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(options.IntervalSeconds),
                stoppingToken);
        }

        _logger.LogInformation("Server check background service stopped");
    }
}