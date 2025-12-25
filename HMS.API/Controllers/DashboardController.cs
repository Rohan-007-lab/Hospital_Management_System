using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Doctor,Receptionist")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _dashboardService.GetDashboardStatsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointmentAnalytics()
    {
        var result = await _dashboardService.GetAppointmentAnalyticsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("revenue")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRevenueAnalytics()
    {
        var result = await _dashboardService.GetRevenueAnalyticsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}