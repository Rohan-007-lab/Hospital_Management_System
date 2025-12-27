using HMS.Web.Models;
using HMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers;

public class LabTestsController : Controller
{
    private readonly ApiService _apiService;

    public LabTestsController(ApiService apiService)
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
            var response = await _apiService.GetAsync<ApiResponse<List<LabTestViewModel>>>("/LabTests");

            if (response != null && response.Success && response.Data != null)
            {
                return View(response.Data);
            }

            return View(new List<LabTestViewModel>());
        }
        catch (Exception)
        {
            return View(new List<LabTestViewModel>());
        }
    }

    public async Task<IActionResult> Create()
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

        var patients = await _apiService.GetAsync<ApiResponse<List<PatientViewModel>>>("/Patients");
        ViewBag.Patients = patients?.Data ?? new List<PatientViewModel>();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLabTestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var patients = await _apiService.GetAsync<ApiResponse<List<PatientViewModel>>>("/Patients");
            ViewBag.Patients = patients?.Data ?? new List<PatientViewModel>();
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync<ApiResponse<LabTestViewModel>>("/LabTests", model);

            if (response != null && response.Success)
            {
                TempData["SuccessMessage"] = "Lab test requested successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = response?.Message ?? "Failed to request lab test";
                return View(model);
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error: {ex.Message}";
            return View(model);
        }
    }
}