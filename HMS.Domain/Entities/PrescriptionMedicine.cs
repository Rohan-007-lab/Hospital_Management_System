using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class PrescriptionMedicine : BaseEntity
{
    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;

    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;

    public string Dosage { get; set; } = string.Empty; // e.g., "500mg"
    public string Frequency { get; set; } = string.Empty; // e.g., "3 times a day"
    public int DurationDays { get; set; }
    public string Instructions { get; set; } = string.Empty; // e.g., "After meals"
    public int Quantity { get; set; }
}
