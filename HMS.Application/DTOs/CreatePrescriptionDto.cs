using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Prescription;

public class CreatePrescriptionDto
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string GeneralInstructions { get; set; } = string.Empty;
    public string? FollowUpDate { get; set; }
    public List<CreatePrescriptionMedicineDto> Medicines { get; set; } = new();
}