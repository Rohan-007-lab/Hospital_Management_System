namespace HMS.Web.Models;

public class BedViewModel
{
    public int Id { get; set; }
    public int WardId { get; set; }
    public string WardName { get; set; } = string.Empty;
    public string BedNumber { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
    public int? CurrentPatientId { get; set; }
    public string? CurrentPatientName { get; set; }
    public DateTime? OccupiedFrom { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
}