using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Medicine;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IMedicineService
{
    Task<ApiResponse<MedicineDto>> GetMedicineByIdAsync(int id);
    Task<ApiResponse<List<MedicineDto>>> GetAllMedicinesAsync();
    Task<ApiResponse<List<MedicineDto>>> GetLowStockMedicinesAsync();
    Task<ApiResponse<List<MedicineDto>>> SearchMedicinesAsync(string searchTerm);
    Task<ApiResponse<MedicineDto>> CreateMedicineAsync(CreateMedicineDto dto);
    Task<ApiResponse<MedicineDto>> UpdateMedicineAsync(int id, CreateMedicineDto dto);
    Task<ApiResponse<bool>> DeleteMedicineAsync(int id);
    Task<ApiResponse<MedicineDto>> UpdateStockAsync(int id, int quantity);
}