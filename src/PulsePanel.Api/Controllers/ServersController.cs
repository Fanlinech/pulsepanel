using Microsoft.AspNetCore.Mvc;
using PulsePanel.Core.DTOs.Common;
using PulsePanel.Core.DTOs.Servers;
using PulsePanel.Core.Interfaces;
using System.Net;


namespace PulsePanel.Api.Controllers
{
    [ApiController]
    [Route("api/servers")]
    public class ServersController : ControllerBase
    {
        private readonly IServerService _serverService;

        private NotFoundObjectResult ServerNotFound()
        {
            var error = new ErrorResponse
            {
                Message = "Server not found",
                StatusCode = HttpStatusCode.NotFound,
                TimeStamp = DateTime.UtcNow,
                Path = HttpContext.Request.Path
            };

            return NotFound(error);
        }
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
                return ServerNotFound();
            }
            return Ok(server);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetServersRequest request)
        {
            var servers = await _serverService.GetAllAsync(request);

            return Ok(servers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServerById(Guid id)
        {
            var server = await _serverService.GetByIDAsync(id);
            if (server == null)
            {
                return ServerNotFound();
            }
            return Ok(server);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsyncServer(Guid id, [FromBody] UpdateServerRequest request)
        {
            var server = await _serverService.UpdateAsync(id, request);
            if (server == null)
            {
                return ServerNotFound();
            }
            return Ok(server);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteServerAsync(Guid id)
        {
            if (await _serverService.DeleteAsync(id)) { return NoContent();}
            return ServerNotFound();
        }

    }
}
