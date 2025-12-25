using AutoMapper;
using HMS.Application.DTOs.Appointment;
using HMS.Application.DTOs.Bed;
using HMS.Application.DTOs.Bill;
using HMS.Application.DTOs.Doctor;
using HMS.Application.DTOs.LabTest;
using HMS.Application.DTOs.Medicine;
using HMS.Application.DTOs.Patient;
using HMS.Application.DTOs.Prescription;
using HMS.Application.DTOs.User;
using HMS.Application.DTOs.Ward;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<User, UserDto>();

        // Patient Mappings
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>();

        // Doctor Mappings
        CreateMap<Doctor, DoctorDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<CreateDoctorDto, Doctor>()
           // .ForMember(dest => dest.WorkingDays, opt => opt.MapFrom(src => SerializeWorkingDays(src.WorkingDays)));
        .ForMember(dest => dest.WorkingDays, opt => opt.Ignore());

        // Appointment Mappings
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.User.FirstName} {src.Patient.User.LastName}"))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.User.FirstName} {src.Doctor.User.LastName}"));
        CreateMap<CreateAppointmentDto, Appointment>();
        CreateMap<UpdateAppointmentDto, Appointment>();

        // Medicine Mappings
        CreateMap<Medicine, MedicineDto>();
        CreateMap<CreateMedicineDto, Medicine>();

        // Prescription Mappings
        CreateMap<Prescription, PrescriptionDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.User.FirstName} {src.Patient.User.LastName}"))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.User.FirstName} {src.Doctor.User.LastName}"))
            .ForMember(dest => dest.Medicines, opt => opt.MapFrom(src => src.PrescriptionMedicines));
        CreateMap<CreatePrescriptionDto, Prescription>();

        CreateMap<PrescriptionMedicine, PrescriptionMedicineDto>()
            .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.MedicineName));
        CreateMap<CreatePrescriptionMedicineDto, PrescriptionMedicine>();

        // LabTest Mappings
        CreateMap<LabTest, LabTestDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.User.FirstName} {src.Patient.User.LastName}"));
        CreateMap<CreateLabTestDto, LabTest>();

        // Bill Mappings
        CreateMap<Bill, BillDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.User.FirstName} {src.Patient.User.LastName}"))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.BillItems));
        CreateMap<CreateBillDto, Bill>();

        CreateMap<BillItem, BillItemDto>();
        CreateMap<CreateBillItemDto, BillItem>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

        // Ward Mappings
        CreateMap<Ward, WardDto>();
        CreateMap<CreateWardDto, Ward>();

        // Bed Mappings
        CreateMap<Bed, BedDto>()
            .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Ward.WardName))
            .ForMember(dest => dest.CurrentPatientName, opt => opt.MapFrom(src =>
                src.CurrentPatientId.HasValue ? "Patient Name" : null));
        CreateMap<CreateBedDto, Bed>();


        CreateMap<Ward, WardDto>();
        CreateMap<CreateWardDto, Ward>();

        // Bed Mappings
        CreateMap<Bed, BedDto>()
            .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Ward.WardName))
            .ForMember(dest => dest.CurrentPatientName, opt => opt.MapFrom(src =>
                src.CurrentPatientId.HasValue && src.Ward != null ? "Patient" : null));
        CreateMap<CreateBedDto, Bed>();


    }
}