using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreatePrescriptionViewModel
{
    [Required]
    public int AppointmentId { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Diagnosis { get; set; } = string.Empty;

    [StringLength(2000)]
    public string GeneralInstructions { get; set; } = string.Empty;

    public string? FollowUpDate { get; set; }

    [Required]
    public List<CreatePrescriptionMedicineViewModel> Medicines { get; set; } = new();
}

public class CreatePrescriptionMedicineViewModel
{
    [Required]
    public int MedicineId { get; set; }

    [Required]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    public string Frequency { get; set; } = string.Empty;

    [Required]
    [Range(1, 365)]
    public int DurationDays { get; set; }

    public string Instructions { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; }
}