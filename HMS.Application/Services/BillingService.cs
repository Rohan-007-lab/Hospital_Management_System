using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using HMS.Application.DTOs.Bill;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Shared.Models;

namespace HMS.Application.Services;

public class BillingService : IBillingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BillingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BillDto>> GetBillByIdAsync(int id)
    {
        try
        {
            var bills = await _unitOfWork.Bills.FindAsync(b => b.Id == id);
            var bill = bills.FirstOrDefault();

            if (bill == null)
            {
                return ApiResponse<BillDto>.FailureResponse("Bill not found");
            }

            await LoadNavigationPropertiesAsync(bill);

            var billDto = _mapper.Map<BillDto>(bill);
            return ApiResponse<BillDto>.SuccessResponse(billDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<BillDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BillDto>>> GetAllBillsAsync()
    {
        try
        {
            var bills = await _unitOfWork.Bills.GetAllAsync();
            var billsList = bills.ToList();

            foreach (var bill in billsList)
            {
                await LoadNavigationPropertiesAsync(bill);
            }

            var billDtos = _mapper.Map<List<BillDto>>(billsList);
            return ApiResponse<List<BillDto>>.SuccessResponse(billDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BillDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BillDto>>> GetBillsByPatientIdAsync(int patientId)
    {
        try
        {
            var bills = await _unitOfWork.Bills.FindAsync(b => b.PatientId == patientId);
            var billsList = bills.ToList();

            foreach (var bill in billsList)
            {
                await LoadNavigationPropertiesAsync(bill);
            }

            var billDtos = _mapper.Map<List<BillDto>>(billsList);
            return ApiResponse<List<BillDto>>.SuccessResponse(billDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BillDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BillDto>>> GetPendingBillsAsync()
    {
        try
        {
            var bills = await _unitOfWork.Bills.FindAsync(b =>
                b.PaymentStatus == PaymentStatus.Pending ||
                b.PaymentStatus == PaymentStatus.PartiallyPaid);
            var billsList = bills.ToList();

            foreach (var bill in billsList)
            {
                await LoadNavigationPropertiesAsync(bill);
            }

            var billDtos = _mapper.Map<List<BillDto>>(billsList);
            return ApiResponse<List<BillDto>>.SuccessResponse(billDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BillDto>>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BillDto>> CreateBillAsync(CreateBillDto dto)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId);
            if (patient == null)
            {
                return ApiResponse<BillDto>.FailureResponse("Patient not found");
            }

            await _unitOfWork.BeginTransactionAsync();

            // Generate bill number
            var billCount = await _unitOfWork.Bills.CountAsync();
            var billNumber = $"BILL{DateTime.UtcNow:yyyyMMdd}{(billCount + 1):D4}";

            // Calculate totals
            decimal subTotal = 0;
            var billItems = new List<BillItem>();

            foreach (var itemDto in dto.Items)
            {
                var totalPrice = itemDto.Quantity * itemDto.UnitPrice;
                subTotal += totalPrice;

                var billItem = new BillItem
                {
                    ItemType = itemDto.ItemType,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice,
                    ReferenceId = itemDto.ReferenceId
                };

                billItems.Add(billItem);
            }

            var totalAmount = subTotal + dto.TaxAmount - dto.Discount;

            var bill = new Bill
            {
                PatientId = dto.PatientId,
                BillNumber = billNumber,
                BillDate = DateTime.UtcNow,
                SubTotal = subTotal,
                TaxAmount = dto.TaxAmount,
                Discount = dto.Discount,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                BalanceAmount = totalAmount,
                PaymentStatus = PaymentStatus.Pending,
                Notes = dto.Notes
            };

            await _unitOfWork.Bills.AddAsync(bill);
            await _unitOfWork.SaveChangesAsync();

            // Add bill items
            foreach (var billItem in billItems)
            {
                billItem.BillId = bill.Id;
                await _unitOfWork.BillItems.AddAsync(billItem);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            await LoadNavigationPropertiesAsync(bill);

            var billDto = _mapper.Map<BillDto>(bill);
            return ApiResponse<BillDto>.SuccessResponse(billDto, "Bill created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<BillDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BillDto>> ProcessPaymentAsync(int billId, decimal amount, string paymentMethod)
    {
        try
        {
            var bills = await _unitOfWork.Bills.FindAsync(b => b.Id == billId);
            var bill = bills.FirstOrDefault();

            if (bill == null)
            {
                return ApiResponse<BillDto>.FailureResponse("Bill not found");
            }

            if (amount <= 0)
            {
                return ApiResponse<BillDto>.FailureResponse("Invalid payment amount");
            }

            if (amount > bill.BalanceAmount)
            {
                return ApiResponse<BillDto>.FailureResponse("Payment amount exceeds balance");
            }

            await _unitOfWork.BeginTransactionAsync();

            // Create payment record
            var paymentCount = await _unitOfWork.Payments.CountAsync();
            var paymentNumber = $"PAY{DateTime.UtcNow:yyyyMMdd}{(paymentCount + 1):D4}";

            var payment = new Payment
            {
                BillId = bill.Id,
                PaymentNumber = paymentNumber,
                PaymentDate = DateTime.UtcNow,
                Amount = amount,
                PaymentMethod = paymentMethod,
                TransactionId = Guid.NewGuid().ToString()
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // Update bill
            bill.PaidAmount += amount;
            bill.BalanceAmount -= amount;

            if (bill.BalanceAmount == 0)
            {
                bill.PaymentStatus = PaymentStatus.Paid;
            }
            else if (bill.PaidAmount > 0)
            {
                bill.PaymentStatus = PaymentStatus.PartiallyPaid;
            }

            bill.PaymentMethod = paymentMethod;
            bill.TransactionId = payment.TransactionId;
            bill.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Bills.UpdateAsync(bill);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            await LoadNavigationPropertiesAsync(bill);

            var billDto = _mapper.Map<BillDto>(bill);
            return ApiResponse<BillDto>.SuccessResponse(billDto, "Payment processed successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<BillDto>.FailureResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteBillAsync(int id)
    {
        try
        {
            var bills = await _unitOfWork.Bills.FindAsync(b => b.Id == id);
            var bill = bills.FirstOrDefault();

            if (bill == null)
            {
                return ApiResponse<bool>.FailureResponse("Bill not found");
            }

            bill.IsDeleted = true;
            bill.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Bills.UpdateAsync(bill);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Bill deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error: {ex.Message}");
        }
    }

    private async Task LoadNavigationPropertiesAsync(Bill bill)
    {
        var patients = await _unitOfWork.Patients.FindAsync(p => p.Id == bill.PatientId);
        bill.Patient = patients.FirstOrDefault()!;

        if (bill.Patient != null)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Id == bill.Patient.UserId);
            bill.Patient.User = users.FirstOrDefault()!;
        }

        var billItems = await _unitOfWork.BillItems.FindAsync(bi => bi.BillId == bill.Id);
        bill.BillItems = billItems.ToList();

        var payments = await _unitOfWork.Payments.FindAsync(p => p.BillId == bill.Id);
        bill.Payments = payments.ToList();
    }
}