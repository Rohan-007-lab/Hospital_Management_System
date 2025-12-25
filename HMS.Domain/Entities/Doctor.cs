using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class Doctor : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string DoctorNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal ConsultationFee { get; set; }
    public string WorkingDays { get; set; } = string.Empty; // JSON: ["Monday", "Tuesday", ...]
    public TimeSpan WorkingHoursStart { get; set; }
    public TimeSpan WorkingHoursEnd { get; set; }
    public int ConsultationDurationMinutes { get; set; } = 30;

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}