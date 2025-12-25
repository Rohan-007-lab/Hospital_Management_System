using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Medicine;

public class CreateMedicineDto
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
}