using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Prescription;

public class CreatePrescriptionMedicineDto
{
    public int MedicineId { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public int Quantity { get; set; }
}