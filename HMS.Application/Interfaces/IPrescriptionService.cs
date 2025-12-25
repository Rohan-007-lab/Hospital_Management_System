using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Prescription;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IPrescriptionService
{
    Task<ApiResponse<PrescriptionDto>> GetPrescriptionByIdAsync(int id);
    Task<ApiResponse<List<PrescriptionDto>>> GetAllPrescriptionsAsync();
    Task<ApiResponse<List<PrescriptionDto>>> GetPrescriptionsByPatientIdAsync(int patientId);
    Task<ApiResponse<List<PrescriptionDto>>> GetPrescriptionsByDoctorIdAsync(int doctorId);
    Task<ApiResponse<PrescriptionDto>> GetPrescriptionByAppointmentIdAsync(int appointmentId);
    Task<ApiResponse<PrescriptionDto>> CreatePrescriptionAsync(CreatePrescriptionDto dto);
    Task<ApiResponse<bool>> DeletePrescriptionAsync(int id);
}