using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.LabTest;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface ILabTestService
{
    Task<ApiResponse<LabTestDto>> GetLabTestByIdAsync(int id);
    Task<ApiResponse<List<LabTestDto>>> GetAllLabTestsAsync();
    Task<ApiResponse<List<LabTestDto>>> GetLabTestsByPatientIdAsync(int patientId);
    Task<ApiResponse<List<LabTestDto>>> GetLabTestsByStatusAsync(string status);
    Task<ApiResponse<LabTestDto>> CreateLabTestAsync(CreateLabTestDto dto);
    Task<ApiResponse<LabTestDto>> UpdateLabTestStatusAsync(int id, string status);
    Task<ApiResponse<LabTestDto>> UpdateLabTestResultsAsync(int id, string results);
    Task<ApiResponse<bool>> DeleteLabTestAsync(int id);
}