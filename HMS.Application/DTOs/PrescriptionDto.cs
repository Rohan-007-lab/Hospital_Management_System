using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Prescription;

public class PrescriptionDto
{
    public int Id { get; set;}
    public int AppointmentId { get; set;}
    public int PatientId { get; set;}
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PrescriptionNumber { get; set; } = string.Empty;
    public DateTime PrescriptionDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string GeneralInstructions { get; set; } = string.Empty;
    public string? FollowUpDate { get; set; }
    public List<PrescriptionMedicineDto> Medicines { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}