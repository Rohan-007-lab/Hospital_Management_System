using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Patient;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IPatientService
{
    Task<ApiResponse<PatientDto>> GetPatientByIdAsync(int id);
    Task<ApiResponse<List<PatientDto>>> GetAllPatientsAsync();
    Task<ApiResponse<PatientDto>> CreatePatientAsync(CreatePatientDto dto);
    Task<ApiResponse<PatientDto>> UpdatePatientAsync(UpdatePatientDto dto);
    Task<ApiResponse<bool>> DeletePatientAsync(int id);
    Task<ApiResponse<PatientDto>> GetPatientByUserIdAsync(int userId);
    Task<ApiResponse<PagedResult<PatientDto>>> GetPatientsPagedAsync(PagedRequest request);
    Task<ApiResponse<List<PatientDto>>> SearchPatientsAsync(string searchTerm);
}