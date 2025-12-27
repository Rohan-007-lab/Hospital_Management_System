using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreateBedViewModel
{
    [Required]
    public int WardId { get; set; }

    [Required]
    [StringLength(20)]
    public string BedNumber { get; set; } = string.Empty;

    public string Status { get; set; } = "Available";
}