using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class Medicine : BaseEntity
{
    public string MedicineName { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; } = 50;
    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }

    // Navigation properties
    public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new List<PrescriptionMedicine>();
}