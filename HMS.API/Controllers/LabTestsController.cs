using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.LabTest;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LabTestsController : ControllerBase
{
    private readonly ILabTestService _labTestService;

    public LabTestsController(ILabTestService labTestService)
    {
        _labTestService = labTestService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,LabTechnician")]
    public async Task<IActionResult> GetAllLabTests()
    {
        var result = await _labTestService.GetAllLabTestsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLabTestById(int id)
    {
        var result = await _labTestService.GetLabTestByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetLabTestsByPatientId(int patientId)
    {
        var result = await _labTestService.GetLabTestsByPatientIdAsync(patientId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,LabTechnician")]
    public async Task<IActionResult> GetLabTestsByStatus(string status)
    {
        var result = await _labTestService.GetLabTestsByStatusAsync(status);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> CreateLabTest([FromBody] CreateLabTestDto dto)
    {
        var result = await _labTestService.CreateLabTestAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetLabTestById), new { id = result.Data!.Id }, result);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,LabTechnician")]
    public async Task<IActionResult> UpdateLabTestStatus(int id, [FromBody] string status)
    {
        var result = await _labTestService.UpdateLabTestStatusAsync(id, status);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPatch("{id}/results")]
    [Authorize(Roles = "Admin,LabTechnician")]
    public async Task<IActionResult> UpdateLabTestResults(int id, [FromBody] string results)
    {
        var result = await _labTestService.UpdateLabTestResultsAsync(id, results);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLabTest(int id)
    {
        var result = await _labTestService.DeleteLabTestAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
