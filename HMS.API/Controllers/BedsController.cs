using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Bed;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BedsController : ControllerBase
{
    private readonly IBedService _bedService;

    public BedsController(IBedService bedService)
    {
        _bedService = bedService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBeds()
    {
        var result = await _bedService.GetAllBedsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBedById(int id)
    {
        var result = await _bedService.GetBedByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("ward/{wardId}")]
    public async Task<IActionResult> GetBedsByWardId(int wardId)
    {
        var result = await _bedService.GetBedsByWardIdAsync(wardId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableBeds()
    {
        var result = await _bedService.GetAvailableBedsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBed([FromBody] CreateBedDto dto)
    {
        var result = await _bedService.CreateBedAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetBedById), new { id = result.Data!.Id }, result);
    }

    [HttpPost("{bedId}/assign/{patientId}")]
    [Authorize(Roles = "Admin,Receptionist,Nurse")]
    public async Task<IActionResult> AssignBedToPatient(int bedId, int patientId)
    {
        var result = await _bedService.AssignBedToPatientAsync(bedId, patientId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{bedId}/release")]
    [Authorize(Roles = "Admin,Receptionist,Nurse")]
    public async Task<IActionResult> ReleaseBed(int bedId)
    {
        var result = await _bedService.ReleaseBedAsync(bedId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBed(int id)
    {
        var result = await _bedService.DeleteBedAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}