using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class Prescription : BaseEntity
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public string PrescriptionNumber { get; set; } = string.Empty;
    public DateTime PrescriptionDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string GeneralInstructions { get; set; } = string.Empty;
    public string? FollowUpDate { get; set; }

    // Navigation properties
    public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new List<PrescriptionMedicine>();
}