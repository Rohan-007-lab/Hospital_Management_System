using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Domain.Entities;

public class Appointment : BaseEntity
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public string AppointmentNumber { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string ReasonForVisit { get; set; } = string.Empty;
    public string? Symptoms { get; set; }
    public string? DoctorNotes { get; set; }
    public string? Diagnosis { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    // Navigation properties
    public Prescription? Prescription { get; set; }
}