using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.DTOs.Medicine;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMedicines()
    {
        var result = await _medicineService.GetAllMedicinesAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicineById(int id)
    {
        var result = await _medicineService.GetMedicineByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> GetLowStockMedicines()
    {
        var result = await _medicineService.GetLowStockMedicinesAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("search/{searchTerm}")]
    public async Task<IActionResult> SearchMedicines(string searchTerm)
    {
        var result = await _medicineService.SearchMedicinesAsync(searchTerm);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> CreateMedicine([FromBody] CreateMedicineDto dto)
    {
        var result = await _medicineService.CreateMedicineAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetMedicineById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> UpdateMedicine(int id, [FromBody] CreateMedicineDto dto)
    {
        var result = await _medicineService.UpdateMedicineAsync(id, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPatch("{id}/stock")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        var result = await _medicineService.UpdateStockAsync(id, quantity);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMedicine(int id)
    {
        var result = await _medicineService.DeleteMedicineAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}