using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class BillItem : BaseEntity
{
    public int BillId { get; set; }
    public Bill Bill { get; set; } = null!;

    public string ItemType { get; set; } = string.Empty; // "Consultation", "Medicine", "LabTest", "Procedure"
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int? ReferenceId { get; set; } // Reference to Appointment, Medicine, LabTest, etc.
}
