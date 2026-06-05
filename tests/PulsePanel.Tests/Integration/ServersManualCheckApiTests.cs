using System.Net;
using System.Net.Http.Json;
using PulsePanel.Core;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.Integration;

public class ServersManualCheckApiTests : IntegrationTestBase
{
    public ServersManualCheckApiTests(PulsePanelApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ManualCheck_ReturnsOnline_WhenPortIsOpen()
    {
        using var listener = StartTcpListener(out var port);
        var server = await CreateServerAsync("Open port", "127.0.0.1", (ushort)port);

        var response = await Client.PostAsync($"/api/servers/{server.Id}/check", null);
        var check = await response.Content.ReadFromJsonAsync<CheckServerResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(check);
        Assert.True(check.IsAvailable);
        Assert.Equal(ServerStatus.Online, check.Status);
        Assert.Equal("Connection successful", check.Message);
    }

    [Fact]
    public async Task ManualCheck_ReturnsOffline_WhenPortIsClosed()
    {
        var server = await CreateServerAsync("Closed port", "127.0.0.1", (ushort)GetClosedPort());

        var response = await Client.PostAsync($"/api/servers/{server.Id}/check", null);
        var check = await response.Content.ReadFromJsonAsync<CheckServerResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(check);
        Assert.False(check.IsAvailable);
        Assert.Equal(ServerStatus.Offline, check.Status);
    }

    [Fact]
    public async Task ManualCheck_ReturnsNotFound_WhenServerDoesNotExist()
    {
        var response = await Client.PostAsync($"/api/servers/{Guid.NewGuid()}/check", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
