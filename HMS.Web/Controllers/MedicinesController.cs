using HMS.Web.Models;
using HMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers;

public class MedicinesController : Controller
{
    private readonly ApiService _apiService;

    public MedicinesController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

        try
        {
            var response = await _apiService.GetAsync<ApiResponse<List<MedicineViewModel>>>("/Medicines");

            if (response != null && response.Success && response.Data != null)
            {
                return View(response.Data);
            }

            return View(new List<MedicineViewModel>());
        }
        catch (Exception)
        {
            return View(new List<MedicineViewModel>());
        }
    }

    public IActionResult Create()
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMedicineViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync<ApiResponse<MedicineViewModel>>("/Medicines", model);

            if (response != null && response.Success)
            {
                TempData["SuccessMessage"] = "Medicine added successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = response?.Message ?? "Failed to add medicine";
                return View(model);
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _apiService.DeleteAsync($"/Medicines/{id}");

            if (success)
            {
                TempData["SuccessMessage"] = "Medicine deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete medicine";
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return RedirectToAction("Index");
        }
    }
}