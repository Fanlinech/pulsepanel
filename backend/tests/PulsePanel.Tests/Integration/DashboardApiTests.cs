using System.Net;
using System.Net.Http.Json;
using PulsePanel.Core.DTOs.Dashboard;

namespace PulsePanel.Tests.Integration;

public class DashboardApiTests : IntegrationTestBase
{
    public DashboardApiTests(PulsePanelApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task DashboardSummary_ReturnsCounts()
    {
        var onlineServer = await CreateServerAsync("Online server", "online.example.com", 443);
        await CreateServerAsync("Unknown server", "unknown.example.com", 443);
        await Client.PostAsync($"/api/servers/{onlineServer.Id}/heartbeat", null);

        var response = await Client.GetAsync("/api/dashboard/summary");
        var summary = await response.Content.ReadFromJsonAsync<DashboardSummaryResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(summary);
        Assert.Equal(2, summary.TotalServers);
        Assert.Equal(1, summary.OnlineServers);
        Assert.Equal(1, summary.OfflineServers);
        Assert.Equal(0, summary.UnknownServers);
    }
}
