using System.Net;
using System.Net.Http.Json;
using PulsePanel.Core.DTOs.Common;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.Integration;

public class ServersCrudApiTests : IntegrationTestBase
{
    public ServersCrudApiTests(PulsePanelApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetServers_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/servers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostServers_CreatesServer()
    {
        var request = CreateRequest("Production API", "api.example.com", 443);

        var response = await Client.PostAsJsonAsync("/api/servers", request);
        var server = await response.Content.ReadFromJsonAsync<ServerResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(server);
        Assert.NotEqual(Guid.Empty, server.Id);
        Assert.Equal(request.Name, server.Name);
        Assert.Equal(request.Host, server.Host);
        Assert.Equal(request.CheckPort, server.CheckPort);
    }

    [Fact]
    public async Task GetServerById_ReturnsCreatedServer()
    {
        var created = await CreateServerAsync("Production API", "api.example.com", 443);

        var response = await Client.GetAsync($"/api/servers/{created.Id}");
        var server = await response.Content.ReadFromJsonAsync<ServerResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(server);
        Assert.Equal(created.Id, server.Id);
        Assert.Equal(created.Name, server.Name);
        Assert.Equal(created.Host, server.Host);
    }

    [Fact]
    public async Task GetServerById_ReturnsNotFound_WhenServerDoesNotExist()
    {
        var id = Guid.NewGuid();

        var response = await Client.GetAsync($"/api/servers/{id}");
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal("Server not found", error.Message);
        Assert.Equal(HttpStatusCode.NotFound, error.StatusCode);
        Assert.Equal($"/api/servers/{id}", error.Path);
    }

    [Fact]
    public async Task PutServer_UpdatesServer()
    {
        var created = await CreateServerAsync("Old name", "old.example.com", 80);
        var request = new UpdateServerRequest
        {
            Name = "New name",
            Host = "new.example.com",
            Description = "Updated server",
            CheckPort = 8443
        };

        var response = await Client.PutAsJsonAsync($"/api/servers/{created.Id}", request);
        var server = await response.Content.ReadFromJsonAsync<ServerResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(server);
        Assert.Equal(created.Id, server.Id);
        Assert.Equal(request.Name, server.Name);
        Assert.Equal(request.Host, server.Host);
        Assert.Equal(request.Description, server.Description);
        Assert.Equal(request.CheckPort, server.CheckPort);
    }

    [Fact]
    public async Task DeleteServer_RemovesServer()
    {
        var created = await CreateServerAsync("Server to delete", "delete.example.com", 80);

        var deleteResponse = await Client.DeleteAsync($"/api/servers/{created.Id}");
        var getResponse = await Client.GetAsync($"/api/servers/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
