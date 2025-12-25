using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces;

public interface IReportService
{
    Task<byte[]> GeneratePrescriptionPdfAsync(int prescriptionId);
    Task<byte[]> GenerateBillPdfAsync(int billId);
    Task<byte[]> GenerateLabReportPdfAsync(int labTestId);
    Task<byte[]> GeneratePatientReportPdfAsync(int patientId);
}