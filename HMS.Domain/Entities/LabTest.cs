using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Domain.Entities;

public class LabTest : BaseEntity
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public string TestNumber { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public LabTestStatus Status { get; set; } = LabTestStatus.Requested;
    public DateTime RequestedDate { get; set; }
    public DateTime? SampleCollectedDate { get; set; }
    public DateTime? ReportDate { get; set; }
    public string? Results { get; set; }
    public string? ReportUrl { get; set; } // PDF report path
    public decimal TestPrice { get; set; }
    public string? TechnicianNotes { get; set; }
    public int? RequestedByDoctorId { get; set; }
}