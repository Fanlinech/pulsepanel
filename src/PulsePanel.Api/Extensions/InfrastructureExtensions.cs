using Microsoft.EntityFrameworkCore;
using PulsePanel.Core.Interfaces;
using PulsePanel.Infrastructure.Persistence;
using PulsePanel.Infrastructure.Services;
using PulsePanel.Core.Services;
using PulsePanel.Core.Options;
using PulsePanel.Infrastructure.BackgroundServices;
namespace PulsePanel.Api.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.Configure<ServerCheckOptions>(
            configuration.GetSection("ServerChecks"));

        services.AddHostedService<ServerCheckBackgroundService>();

        services.AddScoped<IServerService, ServerService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IServerCheckService, ServerCheckService>();
        services.AddSingleton<ServerStatusCalculator>();
        return services;
    }
}