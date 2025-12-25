using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Bill;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BillsController : ControllerBase
{
    private readonly IBillingService _billingService;

    public BillsController(IBillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> GetAllBills()
    {
        var result = await _billingService.GetAllBillsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBillById(int id)
    {
        var result = await _billingService.GetBillByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetBillsByPatientId(int patientId)
    {
        var result = await _billingService.GetBillsByPatientIdAsync(patientId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> GetPendingBills()
    {
        var result = await _billingService.GetPendingBillsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> CreateBill([FromBody] CreateBillDto dto)
    {
        var result = await _billingService.CreateBillAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetBillById), new { id = result.Data!.Id }, result);
    }

    [HttpPost("{id}/payment")]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> ProcessPayment(int id, [FromBody] PaymentRequestDto request)
    {
        var result = await _billingService.ProcessPaymentAsync(id, request.Amount, request.PaymentMethod);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBill(int id)
    {
        var result = await _billingService.DeleteBillAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}

public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}
