using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Ward;

public class CreateWardDto
{
    public string WardName { get; set; } = string.Empty;
    public string WardType { get; set; } = string.Empty;
    public int TotalBeds { get; set; }
    public string Floor { get; set; } = string.Empty;
    public decimal ChargesPerDay { get; set; }
}