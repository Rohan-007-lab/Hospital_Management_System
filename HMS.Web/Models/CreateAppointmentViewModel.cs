using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreateAppointmentViewModel
{
    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan AppointmentTime { get; set; }

    [Required]
    [StringLength(500)]
    public string ReasonForVisit { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Symptoms { get; set; }
}