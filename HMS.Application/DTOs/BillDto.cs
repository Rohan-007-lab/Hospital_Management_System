using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Enums;

namespace HMS.Application.DTOs.Bill;

public class BillDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string BillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public List<BillItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}