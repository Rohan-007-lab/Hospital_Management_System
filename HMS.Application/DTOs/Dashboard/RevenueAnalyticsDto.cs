using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Dashboard;

public class RevenueAnalyticsDto
{
    public decimal TodayRevenue { get; set; }
    public decimal WeekRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public decimal YearRevenue { get; set; }
    public List<DailyRevenueDto> Last30Days { get; set; } = new();
    public List<RevenueByServiceDto> ByService { get; set; } = new();
}

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
}

public class RevenueByServiceDto
{
    public string ServiceType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}