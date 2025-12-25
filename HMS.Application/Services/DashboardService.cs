using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Dashboard;
using HMS.Application.Interfaces;
using HMS.Domain.Enums;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Patient Stats
            var totalPatients = await _unitOfWork.Patients.CountAsync();
            var activePatients = await _unitOfWork.Patients.CountAsync(p => !p.IsDeleted);

            // Doctor Stats
            var totalDoctors = await _unitOfWork.Doctors.CountAsync();

            // Appointment Stats
            var todayAppointments = await _unitOfWork.Appointments.CountAsync(a =>
                a.AppointmentDate.Date == today);

            var pendingAppointments = await _unitOfWork.Appointments.CountAsync(a =>
                a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed);

            var completedAppointments = await _unitOfWork.Appointments.CountAsync(a =>
                a.Status == AppointmentStatus.Completed);

            // Bed Stats
            var allBeds = await _unitOfWork.Beds.GetAllAsync();
            var totalBeds = allBeds.Count();
            var occupiedBeds = allBeds.Count(b => b.IsOccupied);
            var availableBeds = totalBeds - occupiedBeds;

            // Revenue Stats
            var todayBills = await _unitOfWork.Bills.FindAsync(b => b.BillDate.Date == today);
            var todayRevenue = todayBills.Sum(b => b.TotalAmount);

            var monthBills = await _unitOfWork.Bills.FindAsync(b => b.BillDate >= startOfMonth);
            var monthRevenue = monthBills.Sum(b => b.TotalAmount);

            // Pending Bills
            var pendingBills = await _unitOfWork.Bills.CountAsync(b =>
                b.PaymentStatus == PaymentStatus.Pending ||
                b.PaymentStatus == PaymentStatus.PartiallyPaid);

            // Low Stock Medicines
            var lowStockMedicines = await _unitOfWork.Medicines.CountAsync(m =>
                m.StockQuantity <= m.ReorderLevel);

            // Pending Lab Tests
            var pendingLabTests = await _unitOfWork.LabTests.CountAsync(l =>
                l.Status == LabTestStatus.Requested ||
                l.Status == LabTestStatus.SampleCollected ||
                l.Status == LabTestStatus.InProgress);

            var stats = new DashboardStatsDto
            {
                TotalPatients = totalPatients,
                ActivePatients = activePatients,
                TotalDoctors = totalDoctors,
                TodayAppointments = todayAppointments,
                PendingAppointments = pendingAppointments,
                CompletedAppointments = completedAppointments,
                TotalBeds = totalBeds,
                AvailableBeds = availableBeds,
                OccupiedBeds = occupiedBeds,
                TodayRevenue = todayRevenue,
                MonthRevenue = monthRevenue,
                PendingBills = pendingBills,
                LowStockMedicines = lowStockMedicines,
                PendingLabTests = pendingLabTests
            };

            return ApiResponse<DashboardStatsDto>.SuccessResponse(stats);
        }
        catch (Exception ex)
        {
            return ApiResponse<DashboardStatsDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AppointmentAnalyticsDto>> GetAppointmentAnalyticsAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var todayCount = await _unitOfWork.Appointments.CountAsync(a => a.AppointmentDate.Date == today);
            var weekCount = await _unitOfWork.Appointments.CountAsync(a => a.AppointmentDate.Date >= weekStart);
            var monthCount = await _unitOfWork.Appointments.CountAsync(a => a.AppointmentDate >= monthStart);

            // By Status
            var allAppointments = await _unitOfWork.Appointments.GetAllAsync();
            var byStatus = allAppointments
                .GroupBy(a => a.Status)
                .Select(g => new AppointmentByStatusDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            // Top Doctors
            var appointmentsWithDoctors = allAppointments.ToList();
            var topDoctors = new List<AppointmentByDoctorDto>();

            foreach (var appointment in appointmentsWithDoctors)
            {
                var doctors = await _unitOfWork.Doctors.FindAsync(d => d.Id == appointment.DoctorId);
                var doctor = doctors.FirstOrDefault();

                if (doctor != null)
                {
                    var users = await _unitOfWork.Users.FindAsync(u => u.Id == doctor.UserId);
                    var user = users.FirstOrDefault();

                    if (user != null)
                    {
                        var doctorName = $"{user.FirstName} {user.LastName}";
                        var existing = topDoctors.FirstOrDefault(d => d.DoctorName == doctorName);

                        if (existing != null)
                        {
                            existing.AppointmentCount++;
                        }
                        else
                        {
                            topDoctors.Add(new AppointmentByDoctorDto
                            {
                                DoctorName = doctorName,
                                AppointmentCount = 1
                            });
                        }
                    }
                }
            }

            var analytics = new AppointmentAnalyticsDto
            {
                TodayAppointments = todayCount,
                WeekAppointments = weekCount,
                MonthAppointments = monthCount,
                ByStatus = byStatus,
                TopDoctors = topDoctors.OrderByDescending(d => d.AppointmentCount).Take(5).ToList()
            };

            return ApiResponse<AppointmentAnalyticsDto>.SuccessResponse(analytics);
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentAnalyticsDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<RevenueAnalyticsDto>> GetRevenueAnalyticsAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);
            var last30DaysStart = today.AddDays(-30);

            var todayBills = await _unitOfWork.Bills.FindAsync(b => b.BillDate.Date == today);
            var todayRevenue = todayBills.Sum(b => b.TotalAmount);

            var weekBills = await _unitOfWork.Bills.FindAsync(b => b.BillDate.Date >= weekStart);
            var weekRevenue = weekBills.Sum(b => b.TotalAmount);

            var monthBills = await _unitOfWork.Bills.FindAsync(b => b.BillDate >= monthStart);
            var monthRevenue = monthBills.Sum(b => b.TotalAmount);

            var yearBills = await _unitOfWork.Bills.FindAsync(b => b.BillDate >= yearStart);
            var yearRevenue = yearBills.Sum(b => b.TotalAmount);

            // Last 30 Days
            var last30DaysBills = await _unitOfWork.Bills.FindAsync(b => b.BillDate.Date >= last30DaysStart);
            var dailyRevenue = last30DaysBills
                .GroupBy(b => b.BillDate.Date)
                .Select(g => new DailyRevenueDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(b => b.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            // By Service Type
            var allBillItems = await _unitOfWork.BillItems.GetAllAsync();
            var byService = allBillItems
                .GroupBy(bi => bi.ItemType)
                .Select(g => new RevenueByServiceDto
                {
                    ServiceType = g.Key,
                    Amount = g.Sum(bi => bi.TotalPrice)
                })
                .ToList();

            var analytics = new RevenueAnalyticsDto
            {
                TodayRevenue = todayRevenue,
                WeekRevenue = weekRevenue,
                MonthRevenue = monthRevenue,
                YearRevenue = yearRevenue,
                Last30Days = dailyRevenue,
                ByService = byService
            };

            return ApiResponse<RevenueAnalyticsDto>.SuccessResponse(analytics);
        }
        catch (Exception ex)
        {
            return ApiResponse<RevenueAnalyticsDto>.FailureResponse($"Error: {ex.Message}");
        }
    }
}