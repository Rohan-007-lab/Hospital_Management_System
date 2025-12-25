using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Bed;

public class BedDto
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