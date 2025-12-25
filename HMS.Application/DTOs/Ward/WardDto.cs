using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Ward;

public class WardDto
{
    public int Id { get; set; }
    public string WardName { get; set; } = string.Empty;
    public string WardType { get; set; } = string.Empty;
    public int TotalBeds { get; set; }
    public int AvailableBeds { get; set; }
    public int OccupiedBeds => TotalBeds - AvailableBeds;
    public string Floor { get; set; } = string.Empty;
    public decimal ChargesPerDay { get; set; }
    public DateTime CreatedAt { get; set; }
}
