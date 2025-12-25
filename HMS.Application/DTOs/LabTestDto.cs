using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Application.DTOs.LabTest;

public class LabTestDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TestNumber { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public LabTestStatus Status { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? SampleCollectedDate { get; set; }
    public DateTime? ReportDate { get; set; }
    public string? Results { get; set; }
    public string? ReportUrl { get; set; }
    public decimal TestPrice { get; set; }
    public string? TechnicianNotes { get; set; }
    public int? RequestedByDoctorId { get; set; }
    public DateTime CreatedAt { get; set; }
}