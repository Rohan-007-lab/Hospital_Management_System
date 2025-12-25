using HMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Ward;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class WardService : IWardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<WardDto>> GetWardByIdAsync(int id)
    {
        try
        {
            var ward = await _unitOfWork.Wards.GetByIdAsync(id);
            if (ward == null)
            {
                return ApiResponse<WardDto>.FailureResponse("Ward not found");
            }

            var wardDto = _mapper.Map<WardDto>(ward);
            return ApiResponse<WardDto>.SuccessResponse(wardDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<WardDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<WardDto>>> GetAllWardsAsync()
    {
        try
        {
            var wards = await _unitOfWork.Wards.GetAllAsync();
            var wardDtos = _mapper.Map<List<WardDto>>(wards.ToList());
            return ApiResponse<List<WardDto>>.SuccessResponse(wardDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<WardDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<WardDto>>> GetAvailableWardsAsync()
    {
        try
        {
            var wards = await _unitOfWork.Wards.FindAsync(w => w.AvailableBeds > 0);
            var wardDtos = _mapper.Map<List<WardDto>>(wards.ToList());
            return ApiResponse<List<WardDto>>.SuccessResponse(wardDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<WardDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<WardDto>> CreateWardAsync(CreateWardDto dto)
    {
        try
        {
            var ward = new Ward
            {
                WardName = dto.WardName,
                WardType = dto.WardType,
                TotalBeds = dto.TotalBeds,
                AvailableBeds = dto.TotalBeds,
                Floor = dto.Floor,
                ChargesPerDay = dto.ChargesPerDay
            };

            await _unitOfWork.Wards.AddAsync(ward);
            await _unitOfWork.SaveChangesAsync();

            var wardDto = _mapper.Map<WardDto>(ward);
            return ApiResponse<WardDto>.SuccessResponse(wardDto, "Ward created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<WardDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<WardDto>> UpdateWardAsync(int id, CreateWardDto dto)
    {
        try
        {
            var ward = await _unitOfWork.Wards.GetByIdAsync(id);
            if (ward == null)
            {
                return ApiResponse<WardDto>.FailureResponse("Ward not found");
            }

            ward.WardName = dto.WardName;
            ward.WardType = dto.WardType;
            ward.Floor = dto.Floor;
            ward.ChargesPerDay = dto.ChargesPerDay;

            // Update total beds only if it's being increased
            if (dto.TotalBeds > ward.TotalBeds)
            {
                var difference = dto.TotalBeds - ward.TotalBeds;
                ward.AvailableBeds += difference;
                ward.TotalBeds = dto.TotalBeds;
            }

            ward.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Wards.UpdateAsync(ward);
            await _unitOfWork.SaveChangesAsync();

            var wardDto = _mapper.Map<WardDto>(ward);
            return ApiResponse<WardDto>.SuccessResponse(wardDto, "Ward updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<WardDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteWardAsync(int id)
    {
        try
        {
            var ward = await _unitOfWork.Wards.GetByIdAsync(id);
            if (ward == null)
            {
                return ApiResponse<bool>.FailureResponse("Ward not found");
            }

            // Check if ward has occupied beds
            if (ward.AvailableBeds < ward.TotalBeds)
            {
                return ApiResponse<bool>.FailureResponse("Cannot delete ward with occupied beds");
            }

            ward.IsDeleted = true;
            ward.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Wards.UpdateAsync(ward);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Ward deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }
}