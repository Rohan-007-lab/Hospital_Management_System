using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Prescription;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PrescriptionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PrescriptionDto>> GetPrescriptionByIdAsync(int id)
    {
        try
        {
            var prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.Id == id);
            var prescription = prescriptions.FirstOrDefault();

            if (prescription == null)
            {
                return ApiResponse<PrescriptionDto>.FailureResponse("Prescription not found");
            }

            await LoadNavigationPropertiesAsync(prescription);

            var prescriptionDto = _mapper.Map<PrescriptionDto>(prescription);
            return ApiResponse<PrescriptionDto>.SuccessResponse(prescriptionDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PrescriptionDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PrescriptionDto>>> GetAllPrescriptionsAsync()
    {
        try
        {
            var prescriptions = await _unitOfWork.Prescriptions.GetAllAsync();
            var prescriptionsList = prescriptions.ToList();

            foreach (var prescription in prescriptionsList)
            {
                await LoadNavigationPropertiesAsync(prescription);
            }

            var prescriptionDtos = _mapper.Map<List<PrescriptionDto>>(prescriptionsList);
            return ApiResponse<List<PrescriptionDto>>.SuccessResponse(prescriptionDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PrescriptionDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PrescriptionDto>>> GetPrescriptionsByPatientIdAsync(int patientId)
    {
        try
        {
            var prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.PatientId == patientId);
            var prescriptionsList = prescriptions.ToList();

            foreach (var prescription in prescriptionsList)
            {
                await LoadNavigationPropertiesAsync(prescription);
            }

            var prescriptionDtos = _mapper.Map<List<PrescriptionDto>>(prescriptionsList);
            return ApiResponse<List<PrescriptionDto>>.SuccessResponse(prescriptionDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PrescriptionDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PrescriptionDto>>> GetPrescriptionsByDoctorIdAsync(int doctorId)
    {
        try
        {
            var prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.DoctorId == doctorId);
            var prescriptionsList = prescriptions.ToList();

            foreach (var prescription in prescriptionsList)
            {
                await LoadNavigationPropertiesAsync(prescription);
            }

            var prescriptionDtos = _mapper.Map<List<PrescriptionDto>>(prescriptionsList);
            return ApiResponse<List<PrescriptionDto>>.SuccessResponse(prescriptionDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PrescriptionDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PrescriptionDto>> GetPrescriptionByAppointmentIdAsync(int appointmentId)
    {
        try
        {
            var prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.AppointmentId == appointmentId);
            var prescription = prescriptions.FirstOrDefault();

            if (prescription == null)
            {
                return ApiResponse<PrescriptionDto>.FailureResponse("Prescription not found");
            }

            await LoadNavigationPropertiesAsync(prescription);

            var prescriptionDto = _mapper.Map<PrescriptionDto>(prescription);
            return ApiResponse<PrescriptionDto>.SuccessResponse(prescriptionDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PrescriptionDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PrescriptionDto>> CreatePrescriptionAsync(CreatePrescriptionDto dto)
    {
        try
        {
            // Check if appointment exists
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
            {
                return ApiResponse<PrescriptionDto>.FailureResponse("Appointment not found");
            }

            // Check if prescription already exists for this appointment
            var existingPrescription = await _unitOfWork.Prescriptions.FirstOrDefaultAsync(p =>
                p.AppointmentId == dto.AppointmentId);
            if (existingPrescription != null)
            {
                return ApiResponse<PrescriptionDto>.FailureResponse("Prescription already exists for this appointment");
            }

            await _unitOfWork.BeginTransactionAsync();

            // Generate prescription number
            var prescriptionCount = await _unitOfWork.Prescriptions.CountAsync();
            var prescriptionNumber = $"PRE{DateTime.UtcNow:yyyyMMdd}{(prescriptionCount + 1):D4}";

            var prescription = new Prescription
            {
                AppointmentId = dto.AppointmentId,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                PrescriptionNumber = prescriptionNumber,
                PrescriptionDate = DateTime.UtcNow,
                Diagnosis = dto.Diagnosis,
                GeneralInstructions = dto.GeneralInstructions,
                FollowUpDate = dto.FollowUpDate
            };

            await _unitOfWork.Prescriptions.AddAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            // Add prescription medicines
            foreach (var medicineDto in dto.Medicines)
            {
                var medicine = await _unitOfWork.Medicines.GetByIdAsync(medicineDto.MedicineId);
                if (medicine == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<PrescriptionDto>.FailureResponse($"Medicine with ID {medicineDto.MedicineId} not found");
                }

                // Check stock
                if (medicine.StockQuantity < medicineDto.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<PrescriptionDto>.FailureResponse($"Insufficient stock for {medicine.MedicineName}");
                }

                var prescriptionMedicine = new PrescriptionMedicine
                {
                    PrescriptionId = prescription.Id,
                    MedicineId = medicineDto.MedicineId,
                    Dosage = medicineDto.Dosage,
                    Frequency = medicineDto.Frequency,
                    DurationDays = medicineDto.DurationDays,
                    Instructions = medicineDto.Instructions,
                    Quantity = medicineDto.Quantity
                };

                await _unitOfWork.PrescriptionMedicines.AddAsync(prescriptionMedicine);

                // Update medicine stock
                medicine.StockQuantity -= medicineDto.Quantity;
                await _unitOfWork.Medicines.UpdateAsync(medicine);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            await LoadNavigationPropertiesAsync(prescription);

            var prescriptionDto = _mapper.Map<PrescriptionDto>(prescription);
            return ApiResponse<PrescriptionDto>.SuccessResponse(prescriptionDto, "Prescription created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<PrescriptionDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeletePrescriptionAsync(int id)
    {
        try
        {
            var prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.Id == id);
            var prescription = prescriptions.FirstOrDefault();

            if (prescription == null)
            {
                return ApiResponse<bool>.FailureResponse("Prescription not found");
            }

            prescription.IsDeleted = true;
            prescription.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Prescriptions.UpdateAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Prescription deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    private async Task LoadNavigationPropertiesAsync(Prescription prescription)
    {
        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == prescription.PatientId);
        prescription.Patient = patients.FirstOrDefault()!;

        if (prescription.Patient != null)
        {
            var patientUsers = await _unitOfWork.Users.FindAsync(u => u.Id == prescription.Patient.UserId);
            prescription.Patient.User = patientUsers.FirstOrDefault()!;
        }

        var doctors = await _unitOfWork.Doctors.FindAsync(d => d.Id == prescription.DoctorId);
        prescription.Doctor = doctors.FirstOrDefault()!;

        if (prescription.Doctor != null)
        {
            var doctorUsers = await _unitOfWork.Users.FindAsync(u => u.Id == prescription.Doctor.UserId);
            prescription.Doctor.User = doctorUsers.FirstOrDefault()!;
        }

        var prescriptionMedicines = await _unitOfWork.PrescriptionMedicines.FindAsync(pm =>
            pm.PrescriptionId == prescription.Id);
        prescription.PrescriptionMedicines = prescriptionMedicines.ToList();

        foreach (var pm in prescription.PrescriptionMedicines)
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(pm.MedicineId);
            pm.Medicine = medicine!;
        }
    }
}