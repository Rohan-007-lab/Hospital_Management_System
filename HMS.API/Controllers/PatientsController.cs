using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Patient;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Nurse,Receptionist")]
    public async Task<IActionResult> GetAllPatients()
    {
        var result = await _patientService.GetAllPatientsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(int id)
    {
        var result = await _patientService.GetPatientByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPatientByUserId(int userId)
    {
        var result = await _patientService.GetPatientByUserIdAsync(userId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto dto)
    {
        var result = await _patientService.CreatePatientAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetPatientById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Receptionist,Patient")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await _patientService.UpdatePatientAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var result = await _patientService.DeletePatientAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}