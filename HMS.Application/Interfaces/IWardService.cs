using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.Application.DTOs.Ward;

using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IWardService
{
    Task<ApiResponse<WardDto>> GetWardByIdAsync(int id);
    Task<ApiResponse<List<WardDto>>> GetAllWardsAsync();
    Task<ApiResponse<List<WardDto>>> GetAvailableWardsAsync();
    Task<ApiResponse<WardDto>> CreateWardAsync(CreateWardDto dto);
    Task<ApiResponse<WardDto>> UpdateWardAsync(int id, CreateWardDto dto);
    Task<ApiResponse<bool>> DeleteWardAsync(int id);
}