using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities;

public class Payment : BaseEntity
{
    public int BillId { get; set; }
    public Bill Bill { get; set; } = null!;

    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // "Cash", "Card", "UPI", "Insurance"
    public string? TransactionId { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? Notes { get; set; }
}