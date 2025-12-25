using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Patient;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services;

public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PatientService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PatientDto>> GetPatientByIdAsync(int id)
    {
        try
        {
            var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == id);
            var patient = patients.FirstOrDefault();

            if (patient == null)
            {
                return ApiResponse<PatientDto>.FailureResponse("Patient not found");
            }

            // Load User navigation property
            var users = await _unitOfWork.Users.FindAsync(u => u.Id == patient.UserId);
            patient.User = users.FirstOrDefault()!;

            var patientDto = _mapper.Map<PatientDto>(patient);
            return ApiResponse<PatientDto>.SuccessResponse(patientDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PatientDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PatientDto>>> GetAllPatientsAsync()
    {
        try
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            var patientsList = patients.ToList();

            // Load User navigation properties
            foreach (var patient in patientsList)
            {
                var users = await _unitOfWork.Users.FindAsync(u => u.Id == patient.UserId);
                patient.User = users.FirstOrDefault()!;
            }

            var patientDtos = _mapper.Map<List<PatientDto>>(patientsList);
            return ApiResponse<List<PatientDto>>.SuccessResponse(patientDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PatientDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PatientDto>> CreatePatientAsync(CreatePatientDto dto)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return ApiResponse<PatientDto>.FailureResponse("Email already exists");
            }

            await _unitOfWork.BeginTransactionAsync();

            // Create User
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
                Role = UserRole.Patient,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate Patient 
            var patientCount = await _unitOfWork.Patients.CountAsync();
            var patientNumber = $"PAT{DateTime.UtcNow:yyyyMMdd}{(patientCount + 1):D4}";

            // Create Patient
            var patient = new Patient
            {
                UserId = user.Id,
                PatientNumber = patientNumber,
                BloodGroup = dto.BloodGroup,
                MedicalHistory = dto.MedicalHistory,
                Allergies = dto.Allergies,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactPhone = dto.EmergencyContactPhone,
                InsuranceProvider = dto.InsuranceProvider,
                InsuranceNumber = dto.InsuranceNumber
            };

            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            patient.User = user;
            var patientDto = _mapper.Map<PatientDto>(patient);
            return ApiResponse<PatientDto>.SuccessResponse(patientDto, "Patient created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<PatientDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PatientDto>> UpdatePatientAsync(UpdatePatientDto dto)
    {
        try
        {
            var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == dto.Id);
            var patient = patients.FirstOrDefault();

            if (patient == null)
            {
                return ApiResponse<PatientDto>.FailureResponse("Patient not found");
            }

            var users = await _unitOfWork.Users.FindAsync(u => u.Id == patient.UserId);
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

            patient.BloodGroup = dto.BloodGroup;
            patient.MedicalHistory = dto.MedicalHistory;
            patient.Allergies = dto.Allergies;
            patient.EmergencyContactName = dto.EmergencyContactName;
            patient.EmergencyContactPhone = dto.EmergencyContactPhone;
            patient.InsuranceProvider = dto.InsuranceProvider;
            patient.InsuranceNumber = dto.InsuranceNumber;
            patient.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Patients.UpdateAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            patient.User = user!;
            var patientDto = _mapper.Map<PatientDto>(patient);
            return ApiResponse<PatientDto>.SuccessResponse(patientDto, "Patient updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PatientDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeletePatientAsync(int id)
    {
        try
        {
            var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == id);
            var patient = patients.FirstOrDefault();

            if (patient == null)
            {
                return ApiResponse<bool>.FailureResponse("Patient not found");
            }

            patient.IsDeleted = true;
            patient.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Patients.UpdateAsync(patient);

            var users = await _unitOfWork.Users.FindAsync(u => u.Id == patient.UserId);
            var user = users.FirstOrDefault();
            if (user != null)
            {
                user.IsDeleted = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Patient deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PatientDto>> GetPatientByUserIdAsync(int userId)
    {
        try
        {
            var patients = await _unitOfWork.Patients.FindAsync(p => p.UserId == userId);
            var patient = patients.FirstOrDefault();

            if (patient == null)
            {
                return ApiResponse<PatientDto>.FailureResponse("Patient not found");
            }

            var users = await _unitOfWork.Users.FindAsync(u => u.Id == patient.UserId);
            patient.User = users.FirstOrDefault()!;

            var patientDto = _mapper.Map<PatientDto>(patient);
            return ApiResponse<PatientDto>.SuccessResponse(patientDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PatientDto>.FailureResponse($"Error: {ex.Message}");
        }
    }
}