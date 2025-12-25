using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.User;
using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Patient;

public class PatientDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PatientNumber { get; set; } = string.Empty;
    public BloodGroup BloodGroup { get; set; }
    public string MedicalHistory { get; set; } = string.Empty;
    public string Allergies { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public string InsuranceNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // User details
    public UserDto? User { get; set; }
}