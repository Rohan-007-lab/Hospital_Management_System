using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreateWardViewModel
{
    [Required]
    [StringLength(100)]
    public string WardName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string WardType { get; set; } = string.Empty;

    [Required]
    [Range(1, 100)]
    public int TotalBeds { get; set; }

    [Required]
    [StringLength(20)]
    public string Floor { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 100000)]
    public decimal ChargesPerDay { get; set; }
}