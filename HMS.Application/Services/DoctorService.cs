using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Doctor;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Shared.Models;
using System.Text.Json;

namespace HMS.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DoctorService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByIdAsync(int id)
    {
        try
        {
            var doctors = await _unitOfWork.Doctors.FindAsync(d => d.Id == id);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                return ApiResponse<DoctorDto>.FailureResponse("Doctor not found");
            }

            var users = await _unitOfWork.Users.FindAsync(u => u.Id == doctor.UserId);
            doctor.User = users.FirstOrDefault()!;

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return ApiResponse<DoctorDto>.SuccessResponse(doctorDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<DoctorDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<DoctorDto>>> GetAllDoctorsAsync()
    {
        try
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            var doctorsList = doctors.ToList();

            foreach (var doctor in doctorsList)
            {
                var users = await _unitOfWork.Users.FindAsync(u => u.Id == doctor.UserId);
                doctor.User = users.FirstOrDefault()!;
            }

            var doctorDtos = _mapper.Map<List<DoctorDto>>(doctorsList);
            return ApiResponse<List<DoctorDto>>.SuccessResponse(doctorDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<DoctorDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DoctorDto>> CreateDoctorAsync(CreateDoctorDto dto)
    {
        try
        {
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return ApiResponse<DoctorDto>.FailureResponse("Email already exists");
            }

            await _unitOfWork.BeginTransactionAsync();

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = dto.Address,
                Role = UserRole.Doctor,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var doctorCount = await _unitOfWork.Doctors.CountAsync();
            var doctorNumber = $"DOC{DateTime.UtcNow:yyyyMMdd}{(doctorCount + 1):D4}";

            var doctor = new Doctor
            {
                UserId = user.Id,
                DoctorNumber = doctorNumber,
                Specialization = dto.Specialization,
                Qualifications = dto.Qualifications,
                LicenseNumber = dto.LicenseNumber,
                ExperienceYears = dto.ExperienceYears,
                ConsultationFee = dto.ConsultationFee,
                WorkingDays = JsonSerializer.Serialize(dto.WorkingDays),
                WorkingHoursStart = dto.WorkingHoursStart,
                WorkingHoursEnd = dto.WorkingHoursEnd,
                ConsultationDurationMinutes = dto.ConsultationDurationMinutes
            };

            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            doctor.User = user;
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return ApiResponse<DoctorDto>.SuccessResponse(doctorDto, "Doctor created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<DoctorDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DoctorDto>> UpdateDoctorAsync(int id, CreateDoctorDto dto)
    {
        try
        {
            var doctors = await _unitOfWork.Doctors.FindAsync(d => d.Id == id);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                return ApiResponse<DoctorDto>.FailureResponse("Doctor not found");
            }

            var users = await _unitOfWork.Users.FindAsync(u => u.Id == doctor.UserId);
            var user = users.FirstOrDefault();

            if (user != null)
            {
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.PhoneNumber = dto.PhoneNumber;
                user.Address = dto.Address;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
            }

            doctor.Specialization = dto.Specialization;
            doctor.Qualifications = dto.Qualifications;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.ExperienceYears = dto.ExperienceYears;
            doctor.ConsultationFee = dto.ConsultationFee;
            doctor.WorkingDays = JsonSerializer.Serialize(dto.WorkingDays);
            doctor.WorkingHoursStart = dto.WorkingHoursStart;
            doctor.WorkingHoursEnd = dto.WorkingHoursEnd;
            doctor.ConsultationDurationMinutes = dto.ConsultationDurationMinutes;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            doctor.User = user!;
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return ApiResponse<DoctorDto>.SuccessResponse(doctorDto, "Doctor updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DoctorDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteDoctorAsync(int id)
    {
        try
        {
            var doctors = await _unitOfWork.Doctors.FindAsync(d => d.Id == id);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                return ApiResponse<bool>.FailureResponse("Doctor not found");
            }

            doctor.IsDeleted = true;
            doctor.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Doctors.UpdateAsync(doctor);

            var users = await _unitOfWork.Users.FindAsync(u => u.Id == doctor.UserId);
            var user = users.FirstOrDefault();
            if (user != null)
            {
                user.IsDeleted = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Doctor deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByUserIdAsync(int userId)
    {
        try
        {
            var doctors = await _unitOfWork.Doctors.FindAsync(d => d.UserId == userId);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                return ApiResponse<DoctorDto>.FailureResponse("Doctor not found");
            }

            var users = await _unitOfWork.Users.FindAsync(u => u.Id == doctor.UserId);
            doctor.User = users.FirstOrDefault()!;

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return ApiResponse<DoctorDto>.SuccessResponse(doctorDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<DoctorDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<DoctorDto>>> GetDoctorsBySpecializationAsync(string specialization)
    {
        try
        {
            var doctors = await _unitOfWork.Doctors.FindAsync(d =>
                d.Specialization.ToLower().Contains(specialization.ToLower()));
            var doctorsList = doctors.ToList();

            foreach (var doctor in doctorsList)
            {
                var users = await _unitOfWork.Users.FindAsync(u => u.Id == doctor.UserId);
                doctor.User = users.FirstOrDefault()!;
            }

            var doctorDtos = _mapper.Map<List<DoctorDto>>(doctorsList);
            return ApiResponse<List<DoctorDto>>.SuccessResponse(doctorDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<DoctorDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }
}