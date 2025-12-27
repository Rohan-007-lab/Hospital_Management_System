using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreateDoctorViewModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    public string Qualifications { get; set; } = string.Empty;

    [Required]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    [Range(0, 50)]
    public int ExperienceYears { get; set; }

    [Required]
    [Range(0, 100000)]
    public decimal ConsultationFee { get; set; }

    [Required]
    public List<string> WorkingDays { get; set; } = new();

    [Required]
    public TimeSpan WorkingHoursStart { get; set; }

    [Required]
    public TimeSpan WorkingHoursEnd { get; set; }

    public int ConsultationDurationMinutes { get; set; } = 30;
}