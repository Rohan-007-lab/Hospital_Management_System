using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class Ward : BaseEntity
{
    public string WardName { get; set; } = string.Empty;
    public string WardType { get; set; } = string.Empty; // "General", "ICU", "Private", "Semi-Private"
    public int TotalBeds { get; set; }
    public int AvailableBeds { get; set; }
    public string Floor { get; set; } = string.Empty;
    public decimal ChargesPerDay { get; set; }

    // Navigation properties
    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
}