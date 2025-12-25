using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Doctor;

public class CreateDoctorDto
{
    // User Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Address { get; set; } = string.Empty;

    // Doctor Specific Information
    public string Specialization { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal ConsultationFee { get; set; }
    public List<string> WorkingDays { get; set; } = new();
    public TimeSpan WorkingHoursStart { get; set; }
    public TimeSpan WorkingHoursEnd { get; set; }
    public int ConsultationDurationMinutes { get; set; } = 30;
}
