using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models;

public class CreatePatientViewModel
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Blood group is required")]
    public string BloodGroup { get; set; } = string.Empty;

    public string MedicalHistory { get; set; } = string.Empty;

    public string Allergies { get; set; } = string.Empty;

    [Required(ErrorMessage = "Emergency contact name is required")]
    public string EmergencyContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Emergency contact phone is required")]
    public string EmergencyContactPhone { get; set; } = string.Empty;

    public string InsuranceProvider { get; set; } = string.Empty;

    public string InsuranceNumber { get; set; } = string.Empty;
}