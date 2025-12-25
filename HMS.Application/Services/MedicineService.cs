using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Medicine;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class MedicineService : IMedicineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MedicineService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<MedicineDto>> GetMedicineByIdAsync(int id)
    {
        try
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(id);

            if (medicine == null)
            {
                return ApiResponse<MedicineDto>.FailureResponse("Medicine not found");
            }

            var medicineDto = _mapper.Map<MedicineDto>(medicine);
            return ApiResponse<MedicineDto>.SuccessResponse(medicineDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicineDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<MedicineDto>>> GetAllMedicinesAsync()
    {
        try
        {
            var medicines = await _unitOfWork.Medicines.GetAllAsync();
            var medicineDtos = _mapper.Map<List<MedicineDto>>(medicines.ToList());
            return ApiResponse<List<MedicineDto>>.SuccessResponse(medicineDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<MedicineDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<MedicineDto>>> GetLowStockMedicinesAsync()
    {
        try
        {
            var medicines = await _unitOfWork.Medicines.FindAsync(m =>
                m.StockQuantity <= m.ReorderLevel);
            var medicineDtos = _mapper.Map<List<MedicineDto>>(medicines.ToList());
            return ApiResponse<List<MedicineDto>>.SuccessResponse(medicineDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<MedicineDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<MedicineDto>>> SearchMedicinesAsync(string searchTerm)
    {
        try
        {
            var medicines = await _unitOfWork.Medicines.FindAsync(m =>
                m.MedicineName.ToLower().Contains(searchTerm.ToLower()) ||
                m.GenericName.ToLower().Contains(searchTerm.ToLower()));
            var medicineDtos = _mapper.Map<List<MedicineDto>>(medicines.ToList());
            return ApiResponse<List<MedicineDto>>.SuccessResponse(medicineDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<MedicineDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<MedicineDto>> CreateMedicineAsync(CreateMedicineDto dto)
    {
        try
        {
            var medicine = _mapper.Map<Medicine>(dto);
            await _unitOfWork.Medicines.AddAsync(medicine);
            await _unitOfWork.SaveChangesAsync();

            var medicineDto = _mapper.Map<MedicineDto>(medicine);
            return ApiResponse<MedicineDto>.SuccessResponse(medicineDto, "Medicine created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicineDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<MedicineDto>> UpdateMedicineAsync(int id, CreateMedicineDto dto)
    {
        try
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(id);

            if (medicine == null)
            {
                return ApiResponse<MedicineDto>.FailureResponse("Medicine not found");
            }

            medicine.MedicineName = dto.MedicineName;
            medicine.GenericName = dto.GenericName;
            medicine.Manufacturer = dto.Manufacturer;
            medicine.Description = dto.Description;
            medicine.Category = dto.Category;
            medicine.UnitPrice = dto.UnitPrice;
            medicine.StockQuantity = dto.StockQuantity;
            medicine.ReorderLevel = dto.ReorderLevel;
            medicine.ExpiryDate = dto.ExpiryDate;
            medicine.BatchNumber = dto.BatchNumber;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Medicines.UpdateAsync(medicine);
            await _unitOfWork.SaveChangesAsync();

            var medicineDto = _mapper.Map<MedicineDto>(medicine);
            return ApiResponse<MedicineDto>.SuccessResponse(medicineDto, "Medicine updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicineDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteMedicineAsync(int id)
    {
        try
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(id);

            if (medicine == null)
            {
                return ApiResponse<bool>.FailureResponse("Medicine not found");
            }

            medicine.IsDeleted = true;
            medicine.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Medicines.UpdateAsync(medicine);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Medicine deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<MedicineDto>> UpdateStockAsync(int id, int quantity)
    {
        try
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(id);

            if (medicine == null)
            {
                return ApiResponse<MedicineDto>.FailureResponse("Medicine not found");
            }

            medicine.StockQuantity += quantity;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Medicines.UpdateAsync(medicine);
            await _unitOfWork.SaveChangesAsync();

            var medicineDto = _mapper.Map<MedicineDto>(medicine);
            return ApiResponse<MedicineDto>.SuccessResponse(medicineDto, "Stock updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicineDto>.FailureResponse($"Error: {ex.Message}");
        }
    }
}