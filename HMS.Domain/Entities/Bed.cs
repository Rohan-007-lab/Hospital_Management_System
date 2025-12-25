using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class Bed : BaseEntity
{
    public int WardId { get; set; }
    public Ward Ward { get; set; } = null!;

    public string BedNumber { get; set; } = string.Empty;
    public bool IsOccupied { get; set; } = false;
    public int? CurrentPatientId { get; set; }
    public DateTime? OccupiedFrom { get; set; }
    public string? Status { get; set; } // "Available", "Occupied", "Under Maintenance"
}
