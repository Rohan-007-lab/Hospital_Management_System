using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Appointment;

public class AppointmentDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string AppointmentNumber { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
    public string? Symptoms { get; set; }
    public string? DoctorNotes { get; set; }
    public string? Diagnosis { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public DateTime CreatedAt { get; set; }
}