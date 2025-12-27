using HMS.Web.Models;
using HMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers;

public class DoctorsController : Controller
{
    private readonly ApiService _apiService;

    public DoctorsController(ApiService apiService)
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
            var response = await _apiService.GetAsync<ApiResponse<List<DoctorViewModel>>>("/Doctors");

            if (response != null && response.Success && response.Data != null)
            {
                return View(response.Data);
            }

            return View(new List<DoctorViewModel>());
        }
        catch (Exception)
        {
            return View(new List<DoctorViewModel>());
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
    public async Task<IActionResult> Create(CreateDoctorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync<ApiResponse<DoctorViewModel>>("/Doctors", model);

            if (response != null && response.Success)
            {
                TempData["SuccessMessage"] = "Doctor created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = response?.Message ?? "Failed to create doctor";
                return View(model);
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return View(model);
        }
    }

    public async Task<IActionResult> Details(int id)
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
            var response = await _apiService.GetAsync<ApiResponse<DoctorViewModel>>($"/Doctors/{id}");

            if (response != null && response.Success && response.Data != null)
            {
                return View(response.Data);
            }

            return NotFound();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _apiService.DeleteAsync($"/Doctors/{id}");

            if (success)
            {
                TempData["SuccessMessage"] = "Doctor deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete doctor";
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