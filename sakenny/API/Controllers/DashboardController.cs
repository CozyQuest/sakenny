using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.Interfaces;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStatsAsync() => Ok(await _dashboardService.GetStatsAsync());

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueDataAsync([FromQuery] string period = "Y") =>
            Ok(await _dashboardService.GetRevenueChartAsync(period));

        [HttpGet("pendingrequests")]
        public async Task<IActionResult> GetPendingRequests() =>
            Ok(await _dashboardService.GetPendingRequestsAsync());

        [HttpGet("salesbreakdown")]
        public async Task<IActionResult> GetSalesBreakdown([FromQuery] string period = "Y") =>
            Ok(await _dashboardService.GetSalesBreakdownAsync(period));

        [HttpGet("mapmarkers")]
        public async Task<IActionResult> GetMapMarkers() =>
            Ok(await _dashboardService.GetMapMarkersAsync());

        [HttpGet("topproperties")]
        public async Task<IActionResult> GetTopProperties() =>
            Ok(await _dashboardService.GetTopPropertiesAsync());

        [HttpGet("recenttransactions")]
        public async Task<IActionResult> GetRecentTransactions() =>
            Ok(await _dashboardService.GetRecentTransactionsAsync());
    }
}
