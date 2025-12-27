using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreateMedicineViewModel
{
    [Required]
    [StringLength(200)]
    public string MedicineName { get; set; } = string.Empty;

    [StringLength(200)]
    public string GenericName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Manufacturer { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 100000)]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(0, 100000)]
    public int StockQuantity { get; set; }

    [Required]
    [Range(0, 1000)]
    public int ReorderLevel { get; set; } = 50;

    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }

    public string? BatchNumber { get; set; }
}