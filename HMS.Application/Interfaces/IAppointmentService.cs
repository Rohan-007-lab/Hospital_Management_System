using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Appointment;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IAppointmentService
{
    Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id);
    Task<ApiResponse<List<AppointmentDto>>> GetAllAppointmentsAsync();
    Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByPatientIdAsync(int patientId);
    Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(int doctorId);
    Task<ApiResponse<List<AppointmentDto>>> GetAppointmentsByDateAsync(DateTime date);
    Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto);
    Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(UpdateAppointmentDto dto);
    Task<ApiResponse<bool>> CancelAppointmentAsync(int id);
    Task<ApiResponse<AppointmentDto>> CheckInAppointmentAsync(int id);
    Task<ApiResponse<AppointmentDto>> CheckOutAppointmentAsync(int id);
}