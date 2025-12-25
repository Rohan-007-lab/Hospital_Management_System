using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Bill;

public class CreateBillDto
{
    public int PatientId { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
    public List<CreateBillItemDto> Items { get; set; } = new();
}