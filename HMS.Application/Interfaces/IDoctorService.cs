using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Doctor;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IDoctorService
{
    Task<ApiResponse<DoctorDto>> GetDoctorByIdAsync(int id);
    Task<ApiResponse<List<DoctorDto>>> GetAllDoctorsAsync();
    Task<ApiResponse<DoctorDto>> CreateDoctorAsync(CreateDoctorDto dto);
    Task<ApiResponse<DoctorDto>> UpdateDoctorAsync(int id, CreateDoctorDto dto);
    Task<ApiResponse<bool>> DeleteDoctorAsync(int id);
    Task<ApiResponse<DoctorDto>> GetDoctorByUserIdAsync(int userId);
    Task<ApiResponse<List<DoctorDto>>> GetDoctorsBySpecializationAsync(string specialization);
}