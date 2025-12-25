using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Appointment;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Nurse,Receptionist")]
    public async Task<IActionResult> GetAllAppointments()
    {
        var result = await _appointmentService.GetAllAppointmentsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var result = await _appointmentService.GetAppointmentByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetAppointmentsByPatientId(int patientId)
    {
        var result = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetAppointmentsByDoctorId(int doctorId)
    {
        var result = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetAppointmentsByDate(DateTime date)
    {
        var result = await _appointmentService.GetAppointmentsByDateAsync(date);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist,Patient")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
    {
        var result = await _appointmentService.CreateAppointmentAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetAppointmentById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor,Receptionist")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _appointmentService.UpdateAppointmentAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Doctor,Receptionist,Patient")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/checkin")]
    [Authorize(Roles = "Admin,Receptionist,Nurse")]
    public async Task<IActionResult> CheckInAppointment(int id)
    {
        var result = await _appointmentService.CheckInAppointmentAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/checkout")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> CheckOutAppointment(int id)
    {
        var result = await _appointmentService.CheckOutAppointmentAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}