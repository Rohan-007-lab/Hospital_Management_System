using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.LabTest;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class LabTestService : ILabTestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LabTestService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LabTestDto>> GetLabTestByIdAsync(int id)
    {
        try
        {
            var labTests = await _unitOfWork.LabTests.FindAsync(l => l.Id == id);
            var labTest = labTests.FirstOrDefault();

            if (labTest == null)
            {
                return ApiResponse<LabTestDto>.FailureResponse("Lab test not found");
            }

            await LoadNavigationPropertiesAsync(labTest);

            var labTestDto = _mapper.Map<LabTestDto>(labTest);
            return ApiResponse<LabTestDto>.SuccessResponse(labTestDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<LabTestDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<LabTestDto>>> GetAllLabTestsAsync()
    {
        try
        {
            var labTests = await _unitOfWork.LabTests.GetAllAsync();
            var labTestsList = labTests.ToList();

            foreach (var labTest in labTestsList)
            {
                await LoadNavigationPropertiesAsync(labTest);
            }

            var labTestDtos = _mapper.Map<List<LabTestDto>>(labTestsList);
            return ApiResponse<List<LabTestDto>>.SuccessResponse(labTestDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<LabTestDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<LabTestDto>>> GetLabTestsByPatientIdAsync(int patientId)
    {
        try
        {
            var labTests = await _unitOfWork.LabTests.FindAsync(l => l.PatientId == patientId);
            var labTestsList = labTests.ToList();

            foreach (var labTest in labTestsList)
            {
                await LoadNavigationPropertiesAsync(labTest);
            }

            var labTestDtos = _mapper.Map<List<LabTestDto>>(labTestsList);
            return ApiResponse<List<LabTestDto>>.SuccessResponse(labTestDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<LabTestDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<LabTestDto>>> GetLabTestsByStatusAsync(string status)
    {
        try
        {
            var labTestStatus = Enum.Parse<LabTestStatus>(status, true);
            var labTests = await _unitOfWork.LabTests.FindAsync(l => l.Status == labTestStatus);
            var labTestsList = labTests.ToList();

            foreach (var labTest in labTestsList)
            {
                await LoadNavigationPropertiesAsync(labTest);
            }

            var labTestDtos = _mapper.Map<List<LabTestDto>>(labTestsList);
            return ApiResponse<List<LabTestDto>>.SuccessResponse(labTestDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<LabTestDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LabTestDto>> CreateLabTestAsync(CreateLabTestDto dto)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId);
            if (patient == null)
            {
                return ApiResponse<LabTestDto>.FailureResponse("Patient not found");
            }

            var labTestCount = await _unitOfWork.LabTests.CountAsync();
            var testNumber = $"LAB{DateTime.UtcNow:yyyyMMdd}{(labTestCount + 1):D4}";

            var labTest = new LabTest
            {
                PatientId = dto.PatientId,
                TestNumber = testNumber,
                TestName = dto.TestName,
                TestType = dto.TestType,
                Status = LabTestStatus.Requested,
                RequestedDate = DateTime.UtcNow,
                TestPrice = dto.TestPrice,
                RequestedByDoctorId = dto.RequestedByDoctorId
            };

            await _unitOfWork.LabTests.AddAsync(labTest);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(labTest);

            var labTestDto = _mapper.Map<LabTestDto>(labTest);
            return ApiResponse<LabTestDto>.SuccessResponse(labTestDto, "Lab test created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<LabTestDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LabTestDto>> UpdateLabTestStatusAsync(int id, string status)
    {
        try
        {
            var labTests = await _unitOfWork.LabTests.FindAsync(l => l.Id == id);
            var labTest = labTests.FirstOrDefault();

            if (labTest == null)
            {
                return ApiResponse<LabTestDto>.FailureResponse("Lab test not found");
            }

            var labTestStatus = Enum.Parse<LabTestStatus>(status, true);
            labTest.Status = labTestStatus;

            if (labTestStatus == LabTestStatus.SampleCollected)
            {
                labTest.SampleCollectedDate = DateTime.UtcNow;
            }
            else if (labTestStatus == LabTestStatus.ReportReady)
            {
                labTest.ReportDate = DateTime.UtcNow;
            }

            labTest.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.LabTests.UpdateAsync(labTest);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(labTest);

            var labTestDto = _mapper.Map<LabTestDto>(labTest);
            return ApiResponse<LabTestDto>.SuccessResponse(labTestDto, "Status updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<LabTestDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LabTestDto>> UpdateLabTestResultsAsync(int id, string results)
    {
        try
        {
            var labTests = await _unitOfWork.LabTests.FindAsync(l => l.Id == id);
            var labTest = labTests.FirstOrDefault();

            if (labTest == null)
            {
                return ApiResponse<LabTestDto>.FailureResponse("Lab test not found");
            }

            labTest.Results = results;
            labTest.Status = LabTestStatus.Completed;
            labTest.ReportDate = DateTime.UtcNow;
            labTest.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.LabTests.UpdateAsync(labTest);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(labTest);

            var labTestDto = _mapper.Map<LabTestDto>(labTest);
            return ApiResponse<LabTestDto>.SuccessResponse(labTestDto, "Results updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<LabTestDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteLabTestAsync(int id)
    {
        try
        {
            var labTests = await _unitOfWork.LabTests.FindAsync(l => l.Id == id);
            var labTest = labTests.FirstOrDefault();

            if (labTest == null)
            {
                return ApiResponse<bool>.FailureResponse("Lab test not found");
            }

            labTest.IsDeleted = true;
            labTest.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.LabTests.UpdateAsync(labTest);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Lab test deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    private async Task LoadNavigationPropertiesAsync(LabTest labTest)
    {
        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == labTest.PatientId);
        labTest.Patient = patients.FirstOrDefault()!;

        if (labTest.Patient != null)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Id == labTest.Patient.UserId);
            labTest.Patient.User = users.FirstOrDefault()!;
        }
    }
}