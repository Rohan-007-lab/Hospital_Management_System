using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Dashboard;

public class AppointmentAnalyticsDto
{
    public int TodayAppointments { get; set;}
    public int WeekAppointments { get; set;}
    public int MonthAppointments { get; set; }
    public List<AppointmentByStatusDto> ByStatus { get; set; } = new();
    public List<AppointmentByDoctorDto> TopDoctors { get; set; } = new();
}

public class AppointmentByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AppointmentByDoctorDto
{
    public string DoctorName { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
}