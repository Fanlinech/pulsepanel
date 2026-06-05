using System.Net;
using System.Net.Http.Json;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.Integration;

public class ServersSearchSortApiTests : IntegrationTestBase
{
    public ServersSearchSortApiTests(PulsePanelApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetServers_FiltersBySearch()
    {
        await CreateServerAsync("Production API", "prod.example.com", 443, "Main backend");
        await CreateServerAsync("Development API", "dev.example.com", 443, "Sandbox");

        var response = await Client.GetAsync("/api/servers?search=prod");
        var servers = await response.Content.ReadFromJsonAsync<List<ServerResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(servers);
        Assert.Single(servers);
        Assert.Equal("Production API", servers[0].Name);
    }

    [Fact]
    public async Task GetServers_SortsByNameAscending()
    {
        await CreateServerAsync("Charlie", "charlie.example.com", 443);
        await CreateServerAsync("Alpha", "alpha.example.com", 443);
        await CreateServerAsync("Bravo", "bravo.example.com", 443);

        var response = await Client.GetAsync("/api/servers?sortBy=name&sortDirection=asc");
        var servers = await response.Content.ReadFromJsonAsync<List<ServerResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(servers);
        Assert.Equal(new[] { "Alpha", "Bravo", "Charlie" }, servers.Select(server => server.Name));
    }
}
