using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Bill;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IBillingService
{
    Task<ApiResponse<BillDto>> GetBillByIdAsync(int id);
    Task<ApiResponse<List<BillDto>>> GetAllBillsAsync();
    Task<ApiResponse<List<BillDto>>> GetBillsByPatientIdAsync(int patientId);
    Task<ApiResponse<List<BillDto>>> GetPendingBillsAsync();
    Task<ApiResponse<BillDto>> CreateBillAsync(CreateBillDto dto);
    Task<ApiResponse<BillDto>> ProcessPaymentAsync(int billId, decimal amount, string paymentMethod);
    Task<ApiResponse<bool>> DeleteBillAsync(int id);
}