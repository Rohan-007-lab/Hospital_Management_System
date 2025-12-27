namespace HMS.Web.Models;

public class PrescriptionViewModel
{
    public int Id { get; set; }
    public string PrescriptionNumber { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime PrescriptionDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string GeneralInstructions { get; set; } = string.Empty;
    public string? FollowUpDate { get; set; }
    public List<PrescriptionMedicineViewModel> Medicines { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class PrescriptionMedicineViewModel
{
    public int Id { get; set; }
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public int Quantity { get; set; }
}