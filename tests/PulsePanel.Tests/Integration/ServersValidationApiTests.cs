using System.Net;
using System.Net.Http.Json;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.Integration;

public class ServersValidationApiTests : IntegrationTestBase
{
    public ServersValidationApiTests(PulsePanelApiFactory factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("", "api.example.com", 80)]
    [InlineData("A", "api.example.com", 80)]
    [InlineData("Valid name", "", 80)]
    [InlineData("Valid name", "api.example.com", 0)]
    public async Task PostServers_ReturnsBadRequest_WhenRequestIsInvalid(string name, string host, ushort checkPort)
    {
        var request = new CreateServerRequest
        {
            Name = name,
            Host = host,
            CheckPort = checkPort
        };

        var response = await Client.PostAsJsonAsync("/api/servers", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutServer_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        var created = await CreateServerAsync("Production API", "api.example.com", 443);
        var request = new UpdateServerRequest
        {
            Name = "",
            Host = "api.example.com",
            CheckPort = 443
        };

        var response = await Client.PutAsJsonAsync($"/api/servers/{created.Id}", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
