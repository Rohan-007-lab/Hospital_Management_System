using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreateLabTestViewModel
{
    [Required]
    public int PatientId { get; set; }

    [Required]
    [StringLength(200)]
    public string TestName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string TestType { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 100000)]
    public decimal TestPrice { get; set; }

    public int? RequestedByDoctorId { get; set; }
}