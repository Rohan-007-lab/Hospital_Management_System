using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Dashboard;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IDashboardService
{
    Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
    Task<ApiResponse<AppointmentAnalyticsDto>> GetAppointmentAnalyticsAsync();
    Task<ApiResponse<RevenueAnalyticsDto>> GetRevenueAnalyticsAsync();
}