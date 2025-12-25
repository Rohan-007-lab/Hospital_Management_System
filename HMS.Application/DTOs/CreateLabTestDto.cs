using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs.LabTest;

public class CreateLabTestDto
{
    public int PatientId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public decimal TestPrice { get; set; }
    public int? RequestedByDoctorId { get; set; }
}
