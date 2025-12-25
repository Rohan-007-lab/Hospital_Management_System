using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Doctor;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        var result = await _doctorService.GetAllDoctorsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDoctorById(int id)
    {
        var result = await _doctorService.GetDoctorByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetDoctorByUserId(int userId)
    {
        var result = await _doctorService.GetDoctorByUserIdAsync(userId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("specialization/{specialization}")]
    public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
    {
        var result = await _doctorService.GetDoctorsBySpecializationAsync(specialization);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto dto)
    {
        var result = await _doctorService.CreateDoctorAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetDoctorById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> UpdateDoctor(int id, [FromBody] CreateDoctorDto dto)
    {
        var result = await _doctorService.UpdateDoctorAsync(id, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var result = await _doctorService.DeleteDoctorAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}