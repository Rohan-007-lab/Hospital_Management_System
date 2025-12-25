using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.Bed;

public class CreateBedDto
{
    public int WardId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
}