using Microsoft.AspNetCore.Mvc;
using PulsePanel.Core.DTOs.Servers;
using PulsePanel.Core.Interfaces;


namespace PulsePanel.Api.Controllers
{
    [ApiController]
    [Route("api/servers")]
    public class ServersController : ControllerBase
    {
        private readonly IServerService _serverService;
        public ServersController(IServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpPost]
        public async Task<ActionResult<ServerResponse>> CreateServer([FromBody] CreateServerRequest request)
        {
            var server = await _serverService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetServerById),
                new { id = server.Id },
                server);
        }
        [HttpPost("{id:guid}/heartbeat")]
        public async Task<ActionResult<ServerResponse>> UpdateHeartbeat(Guid id)
        {
            var server = await _serverService.UpdateHeartbeatAsync(id);
            if (server == null)
            {
                return NotFound();
            }
            return Ok(server);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServers()
        {
            var servers = await _serverService.GetAllAsync();
            return Ok(servers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServerById(Guid id)
        {
            var server = await _serverService.GetByIDAsync(id);
            if (server == null)
            {
                return NotFound();
            }
            return Ok(server);
        }

    }
}
