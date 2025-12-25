using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Patient;

public class CreatePatientDto
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

    // Patient Specific Information
    public BloodGroup BloodGroup { get; set; }
    public string MedicalHistory { get; set; } = string.Empty;
    public string Allergies { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public string InsuranceNumber { get; set; } = string.Empty;
}