using Microsoft.EntityFrameworkCore;
using PulsePanel.Core.Interfaces;
using PulsePanel.Infrastructure.Persistence;
using PulsePanel.Infrastructure.Services;
using PulsePanel.Core.Services;
namespace PulsePanel.Api.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IServerService, ServerService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddSingleton<ServerStatusCalculator>();
        return services;
    }
}