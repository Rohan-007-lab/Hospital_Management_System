using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Appointment;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Id == id);
            var appointment = appointments.FirstOrDefault();

            if (appointment == null)
            {
                return ApiResponse<AppointmentDto>.FailureResponse("Appointment not found");
            }

            await LoadNavigationPropertiesAsync(appointment);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<AppointmentDto>>> GetAllAppointmentsAsync()
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            var appointmentsList = appointments.ToList();

            foreach (var appointment in appointmentsList)
            {
                await LoadNavigationPropertiesAsync(appointment);
            }

            var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointmentsList);
            return ApiResponse<List<AppointmentDto>>.SuccessResponse(appointmentDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<AppointmentDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByPatientIdAsync(int patientId)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == patientId);
            var appointmentsList = appointments.ToList();

            foreach (var appointment in appointmentsList)
            {
                await LoadNavigationPropertiesAsync(appointment);
            }

            var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointmentsList);
            return ApiResponse<List<AppointmentDto>>.SuccessResponse(appointmentDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<AppointmentDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(int doctorId)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctorId);
            var appointmentsList = appointments.ToList();

            foreach (var appointment in appointmentsList)
            {
                await LoadNavigationPropertiesAsync(appointment);
            }

            var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointmentsList);
            return ApiResponse<List<AppointmentDto>>.SuccessResponse(appointmentDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<AppointmentDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByDateAsync(DateTime date)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a =>
                a.AppointmentDate.Date == date.Date);
            var appointmentsList = appointments.ToList();

            foreach (var appointment in appointmentsList)
            {
                await LoadNavigationPropertiesAsync(appointment);
            }

            var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointmentsList);
            return ApiResponse<List<AppointmentDto>>.SuccessResponse(appointmentDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<AppointmentDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto)
    {
        try
        {
            // Check if patient exists
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId);
            if (patient == null)
            {
                return ApiResponse<AppointmentDto>.FailureResponse("Patient not found");
            }

            // Check if doctor exists
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId);
            if (doctor == null)
            {
                return ApiResponse<AppointmentDto>.FailureResponse("Doctor not found");
            }

            // Check if slot is available
            var existingAppointments = await _unitOfWork.Appointments.FindAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.AppointmentDate.Date == dto.AppointmentDate.Date &&
                a.AppointmentTime == dto.AppointmentTime &&
                a.Status != AppointmentStatus.Cancelled);

            if (existingAppointments.Any())
            {
                return ApiResponse<AppointmentDto>.FailureResponse("Time slot already booked");
            }

            // Generate appointment number
            var appointmentCount = await _unitOfWork.Appointments.CountAsync();
            var appointmentNumber = $"APT{DateTime.UtcNow:yyyyMMdd}{(appointmentCount + 1):D4}";

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentNumber = appointmentNumber,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Status = AppointmentStatus.Scheduled,
                ReasonForVisit = dto.ReasonForVisit,
                Symptoms = dto.Symptoms
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(appointment);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto, "Appointment created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(UpdateAppointmentDto dto)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Id == dto.Id);
            var appointment = appointments.FirstOrDefault();

            if (appointment == null)
            {
                return ApiResponse<AppointmentDto>.FailureResponse("Appointment not found");
            }

            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.AppointmentTime = dto.AppointmentTime;
            appointment.Status = dto.Status;
            appointment.DoctorNotes = dto.DoctorNotes;
            appointment.Diagnosis = dto.Diagnosis;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(appointment);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto, "Appointment updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> CancelAppointmentAsync(int id)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Id == id);
            var appointment = appointments.FirstOrDefault();

            if (appointment == null)
            {
                return ApiResponse<bool>.FailureResponse("Appointment not found");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Appointment cancelled successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AppointmentDto>> CheckInAppointmentAsync(int id)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Id == id);
            var appointment = appointments.FirstOrDefault();

            if (appointment == null)
            {
                return ApiResponse<AppointmentDto>.FailureResponse("Appointment not found");
            }

            appointment.CheckInTime = DateTime.UtcNow;
            appointment.Status = AppointmentStatus.InProgress;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(appointment);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto, "Patient checked in successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AppointmentDto>> CheckOutAppointmentAsync(int id)
    {
        try
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Id == id);
            var appointment = appointments.FirstOrDefault();

            if (appointment == null)
            {
                return ApiResponse<AppointmentDto>.FailureResponse("Appointment not found");
            }

            appointment.CheckOutTime = DateTime.UtcNow;
            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(appointment);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto, "Appointment completed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    private async Task LoadNavigationPropertiesAsync(Appointment appointment)
    {
        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == appointment.PatientId);
        appointment.Patient = patients.FirstOrDefault()!;

        if (appointment.Patient != null)
        {
            var patientUsers = await _unitOfWork.Users.FindAsync(u => u.Id == appointment.Patient.UserId);
            appointment.Patient.User = patientUsers.FirstOrDefault()!;
        }

        var doctors = await _unitOfWork.Doctors.FindAsync(d => d.Id == appointment.DoctorId);
        appointment.Doctor = doctors.FirstOrDefault()!;

        if (appointment.Doctor != null)
        {
            var doctorUsers = await _unitOfWork.Users.FindAsync(u => u.Id == appointment.Doctor.UserId);
            appointment.Doctor.User = doctorUsers.FirstOrDefault()!;
        }
    }
}