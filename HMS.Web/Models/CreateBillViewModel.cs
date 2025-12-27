using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreateBillViewModel
{
    [Required]
    public int PatientId { get; set; }

    [Range(0, 100000)]
    public decimal TaxAmount { get; set; }

    [Range(0, 100000)]
    public decimal Discount { get; set; }

    public string? Notes { get; set; }

    [Required]
    public List<CreateBillItemViewModel> Items { get; set; } = new();
}

public class CreateBillItemViewModel
{
    [Required]
    public string ItemType { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, 100000)]
    public decimal UnitPrice { get; set; }

    public int? ReferenceId { get; set; }
}