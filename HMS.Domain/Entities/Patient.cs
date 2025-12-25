using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Domain.Entities;

public class Patient : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string PatientNumber { get; set; } = string.Empty; // Unique identifier
    public BloodGroup BloodGroup { get; set; }
    public string MedicalHistory { get; set; } = string.Empty;
    public string Allergies { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public string InsuranceNumber { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
}