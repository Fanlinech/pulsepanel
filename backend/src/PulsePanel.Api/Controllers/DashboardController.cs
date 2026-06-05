using Microsoft.AspNetCore.Mvc;
using PulsePanel.Core.Interfaces;

namespace PulsePanel.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboard = await _dashboardService.GetSummaryAsync();
            return Ok(dashboard);
        }
    }
}
