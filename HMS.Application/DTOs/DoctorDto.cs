using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.User;

namespace HMS.Application.DTOs.Doctor;

public class DoctorDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DoctorNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal ConsultationFee { get; set;}
    public string WorkingDays { get; set; } = string.Empty;
    public TimeSpan WorkingHoursStart { get; set; }
    public TimeSpan WorkingHoursEnd { get; set;}
    public int ConsultationDurationMinutes { get; set;}
    public DateTime CreatedAt { get; set; }

    // User details
    public UserDto? User { get; set; }
}