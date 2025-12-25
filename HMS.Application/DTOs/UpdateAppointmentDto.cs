using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Appointment;

public class UpdateAppointmentDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? DoctorNotes { get; set; }
    public string? Diagnosis { get; set; }
}