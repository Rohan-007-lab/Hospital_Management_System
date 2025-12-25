using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Bed;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IBedService
{
    Task<ApiResponse<BedDto>> GetBedByIdAsync(int id);
    Task<ApiResponse<List<BedDto>>> GetAllBedsAsync();
    Task<ApiResponse<List<BedDto>>> GetBedsByWardIdAsync(int wardId);
    Task<ApiResponse<List<BedDto>>> GetAvailableBedsAsync();
    Task<ApiResponse<BedDto>> CreateBedAsync(CreateBedDto dto);
    Task<ApiResponse<BedDto>> AssignBedToPatientAsync(int bedId, int patientId);
    Task<ApiResponse<BedDto>> ReleaseBedAsync(int bedId);
    Task<ApiResponse<bool>> DeleteBedAsync(int id);
}