namespace HMS.Web.Models;

public class AppointmentViewModel
{
    public int Id { get; set; }
    public string AppointmentNumber { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ReasonForVisit { get; set; } = string.Empty;
    public string? Symptoms { get; set; }
    public string? DoctorNotes { get; set; }
    public string? Diagnosis { get; set; }
    public DateTime CreatedAt { get; set; }
}