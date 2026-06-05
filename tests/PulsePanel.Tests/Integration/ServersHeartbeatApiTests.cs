using System.Net;
using System.Net.Http.Json;
using PulsePanel.Core;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.Integration;

public class ServersHeartbeatApiTests : IntegrationTestBase
{
    public ServersHeartbeatApiTests(PulsePanelApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateHeartbeat_UpdatesServerStatus()
    {
        var created = await CreateServerAsync("Heartbeat server", "heartbeat.example.com", 443);

        var response = await Client.PostAsync($"/api/servers/{created.Id}/heartbeat", null);
        var server = await response.Content.ReadFromJsonAsync<ServerResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(server);
        Assert.Equal(created.Id, server.Id);
        Assert.NotNull(server.LastHeartbeatAt);
        Assert.Equal(ServerStatus.Online, server.Status);
    }

    [Fact]
    public async Task UpdateHeartbeat_ReturnsNotFound_WhenServerDoesNotExist()
    {
        var response = await Client.PostAsync($"/api/servers/{Guid.NewGuid()}/heartbeat", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
