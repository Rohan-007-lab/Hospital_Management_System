namespace HMS.Web.Models;

public class LabTestViewModel
{
    public int Id { get; set; }
    public string TestNumber { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public DateTime? SampleCollectedDate { get; set; }
    public DateTime? ReportDate { get; set; }
    public string? Results { get; set; }
    public string? ReportUrl { get; set; }
    public decimal TestPrice { get; set; }
    public string? TechnicianNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}