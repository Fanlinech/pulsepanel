using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using PulsePanel.Core.DTOs.Servers;

namespace PulsePanel.Tests.Integration;

public abstract class IntegrationTestBase : IClassFixture<PulsePanelApiFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client;

    private readonly PulsePanelApiFactory _factory;

    protected IntegrationTestBase(PulsePanelApiFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task<ServerResponse> CreateServerAsync(
        string name,
        string host,
        ushort checkPort,
        string? description = null)
    {
        var response = await Client.PostAsJsonAsync(
            "/api/servers",
            CreateRequest(name, host, checkPort, description));

        response.EnsureSuccessStatusCode();

        var server = await response.Content.ReadFromJsonAsync<ServerResponse>();

        return server!;
    }

    protected static CreateServerRequest CreateRequest(
        string name,
        string host,
        ushort checkPort,
        string? description = null)
    {
        return new CreateServerRequest
        {
            Name = name,
            Host = host,
            Description = description,
            CheckPort = checkPort
        };
    }

    protected static TcpListener StartTcpListener(out int port)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        port = ((IPEndPoint)listener.LocalEndpoint).Port;

        return listener;
    }

    protected static int GetClosedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();

        return port;
    }
}
