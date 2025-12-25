using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Domain.Entities;

public class Bill : BaseEntity
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}