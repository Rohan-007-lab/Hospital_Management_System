using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Bed;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class BedService : IBedService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BedService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BedDto>> GetBedByIdAsync(int id)
    {
        try
        {
            var beds = await _unitOfWork.Beds.FindAsync(b => b.Id == id);
            var bed = beds.FirstOrDefault();

            if (bed == null)
            {
                return ApiResponse<BedDto>.FailureResponse("Bed not found");
            }

            await LoadNavigationPropertiesAsync(bed);

            var bedDto = _mapper.Map<BedDto>(bed);
            return ApiResponse<BedDto>.SuccessResponse(bedDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<BedDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BedDto>>> GetAllBedsAsync()
    {
        try
        {
            var beds = await _unitOfWork.Beds.GetAllAsync();
            var bedsList = beds.ToList();

            foreach (var bed in bedsList)
            {
                await LoadNavigationPropertiesAsync(bed);
            }

            var bedDtos = _mapper.Map<List<BedDto>>(bedsList);
            return ApiResponse<List<BedDto>>.SuccessResponse(bedDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BedDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BedDto>>> GetBedsByWardIdAsync(int wardId)
    {
        try
        {
            var beds = await _unitOfWork.Beds.FindAsync(b => b.WardId == wardId);
            var bedsList = beds.ToList();

            foreach (var bed in bedsList)
            {
                await LoadNavigationPropertiesAsync(bed);
            }

            var bedDtos = _mapper.Map<List<BedDto>>(bedsList);
            return ApiResponse<List<BedDto>>.SuccessResponse(bedDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BedDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BedDto>>> GetAvailableBedsAsync()
    {
        try
        {
            var beds = await _unitOfWork.Beds.FindAsync(b => !b.IsOccupied);
            var bedsList = beds.ToList();

            foreach (var bed in bedsList)
            {
                await LoadNavigationPropertiesAsync(bed);
            }

            var bedDtos = _mapper.Map<List<BedDto>>(bedsList);
            return ApiResponse<List<BedDto>>.SuccessResponse(bedDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BedDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BedDto>> CreateBedAsync(CreateBedDto dto)
    {
        try
        {
            var ward = await _unitOfWork.Wards.GetByIdAsync(dto.WardId);
            if (ward == null)
            {
                return ApiResponse<BedDto>.FailureResponse("Ward not found");
            }

            // Check if bed number already exists in ward
            var existingBed = await _unitOfWork.Beds.FirstOrDefaultAsync(b =>
                b.WardId == dto.WardId && b.BedNumber == dto.BedNumber);

            if (existingBed != null)
            {
                return ApiResponse<BedDto>.FailureResponse("Bed number already exists in this ward");
            }

            var bed = new Bed
            {
                WardId = dto.WardId,
                BedNumber = dto.BedNumber,
                IsOccupied = false,
                Status = dto.Status
            };

            await _unitOfWork.Beds.AddAsync(bed);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(bed);

            var bedDto = _mapper.Map<BedDto>(bed);
            return ApiResponse<BedDto>.SuccessResponse(bedDto, "Bed created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<BedDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BedDto>> AssignBedToPatientAsync(int bedId, int patientId)
    {
        try
        {
            var beds = await _unitOfWork.Beds.FindAsync(b => b.Id == bedId);
            var bed = beds.FirstOrDefault();

            if (bed == null)
            {
                return ApiResponse<BedDto>.FailureResponse("Bed not found");
            }

            if (bed.IsOccupied)
            {
                return ApiResponse<BedDto>.FailureResponse("Bed is already occupied");
            }

            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
            if (patient == null)
            {
                return ApiResponse<BedDto>.FailureResponse("Patient not found");
            }

            await _unitOfWork.BeginTransactionAsync();

            bed.IsOccupied = true;
            bed.CurrentPatientId = patientId;
            bed.OccupiedFrom = DateTime.UtcNow;
            bed.Status = "Occupied";
            bed.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Beds.UpdateAsync(bed);

            // Update ward available beds
            var ward = await _unitOfWork.Wards.GetByIdAsync(bed.WardId);
            if (ward != null)
            {
                ward.AvailableBeds--;
                await _unitOfWork.Wards.UpdateAsync(ward);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            await LoadNavigationPropertiesAsync(bed);

            var bedDto = _mapper.Map<BedDto>(bed);
            return ApiResponse<BedDto>.SuccessResponse(bedDto, "Bed assigned successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<BedDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BedDto>> ReleaseBedAsync(int bedId)
    {
        try
        {
            var beds = await _unitOfWork.Beds.FindAsync(b => b.Id == bedId);
            var bed = beds.FirstOrDefault();

            if (bed == null)
            {
                return ApiResponse<BedDto>.FailureResponse("Bed not found");
            }

            if (!bed.IsOccupied)
            {
                return ApiResponse<BedDto>.FailureResponse("Bed is not occupied");
            }

            await _unitOfWork.BeginTransactionAsync();

            bed.IsOccupied = false;
            bed.CurrentPatientId = null;
            bed.OccupiedFrom = null;
            bed.Status = "Available";
            bed.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Beds.UpdateAsync(bed);

            // Update ward available beds
            var ward = await _unitOfWork.Wards.GetByIdAsync(bed.WardId);
            if (ward != null)
            {
                ward.AvailableBeds++;
                await _unitOfWork.Wards.UpdateAsync(ward);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            await LoadNavigationPropertiesAsync(bed);

            var bedDto = _mapper.Map<BedDto>(bed);
            return ApiResponse<BedDto>.SuccessResponse(bedDto, "Bed released successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<BedDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteBedAsync(int id)
    {
        try
        {
            var beds = await _unitOfWork.Beds.FindAsync(b => b.Id == id);
            var bed = beds.FirstOrDefault();

            if (bed == null)
            {
                return ApiResponse<bool>.FailureResponse("Bed not found");
            }

            if (bed.IsOccupied)
            {
                return ApiResponse<bool>.FailureResponse("Cannot delete occupied bed");
            }

            bed.IsDeleted = true;
            bed.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Beds.UpdateAsync(bed);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Bed deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    private async Task LoadNavigationPropertiesAsync(Bed bed)
    {
        var ward = await _unitOfWork.Wards.GetByIdAsync(bed.WardId);
        bed.Ward = ward!;

        if (bed.CurrentPatientId.HasValue)
        {
            var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == bed.CurrentPatientId.Value);
            var patient = patients.FirstOrDefault();

            if (patient != null)
            {
                var users = await _unitOfWork.Users.FindAsync(u => u.Id == patient.UserId);
                patient.User = users.FirstOrDefault()!;
            }
        }
    }
}