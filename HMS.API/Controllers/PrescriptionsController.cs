using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Prescription;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;

    public PrescriptionsController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Pharmacist")]
    public async Task<IActionResult> GetAllPrescriptions()
    {
        var result = await _prescriptionService.GetAllPrescriptionsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrescriptionById(int id)
    {
        var result = await _prescriptionService.GetPrescriptionByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPrescriptionsByPatientId(int patientId)
    {
        var result = await _prescriptionService.GetPrescriptionsByPatientIdAsync(patientId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetPrescriptionsByDoctorId(int doctorId)
    {
        var result = await _prescriptionService.GetPrescriptionsByDoctorIdAsync(doctorId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("appointment/{appointmentId}")]
    public async Task<IActionResult> GetPrescriptionByAppointmentId(int appointmentId)
    {
        var result = await _prescriptionService.GetPrescriptionByAppointmentIdAsync(appointmentId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDto dto)
    {
        var result = await _prescriptionService.CreatePrescriptionAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetPrescriptionById), new { id = result.Data!.Id }, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePrescription(int id)
    {
        var result = await _prescriptionService.DeletePrescriptionAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
