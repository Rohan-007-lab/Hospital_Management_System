using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Ward;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WardsController : ControllerBase
{
    private readonly IWardService _wardService;

    public WardsController(IWardService wardService)
    {
        _wardService = wardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWards()
    {
        var result = await _wardService.GetAllWardsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWardById(int id)
    {
        var result = await _wardService.GetWardByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableWards()
    {
        var result = await _wardService.GetAvailableWardsAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateWard([FromBody] CreateWardDto dto)
    {
        var result = await _wardService.CreateWardAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetWardById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateWard(int id, [FromBody] CreateWardDto dto)
    {
        var result = await _wardService.UpdateWardAsync(id, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteWard(int id)
    {
        var result = await _wardService.DeleteWardAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}