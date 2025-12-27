using HMS.Web.Models;
using HMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers;

public class AppointmentsController : Controller
{
    private readonly ApiService _apiService;

    public AppointmentsController(ApiService apiService)
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
            var response = await _apiService.GetAsync<ApiResponse<List<AppointmentViewModel>>>("/Appointments");

            if (response != null && response.Success && response.Data != null)
            {
                return View(response.Data);
            }

            return View(new List<AppointmentViewModel>());
        }
        catch (Exception)
        {
            return View(new List<AppointmentViewModel>());
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

        // Load Patients and Doctors for dropdowns
        var patients = await _apiService.GetAsync<ApiResponse<List<PatientViewModel>>>("/Patients");
        var doctors = await _apiService.GetAsync<ApiResponse<List<DoctorViewModel>>>("/Doctors");

        ViewBag.Patients = patients?.Data ?? new List<PatientViewModel>();
        ViewBag.Doctors = doctors?.Data ?? new List<DoctorViewModel>();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var patients = await _apiService.GetAsync<ApiResponse<List<PatientViewModel>>>("/Patients");
            var doctors = await _apiService.GetAsync<ApiResponse<List<DoctorViewModel>>>("/Doctors");
            ViewBag.Patients = patients?.Data ?? new List<PatientViewModel>();
            ViewBag.Doctors = doctors?.Data ?? new List<DoctorViewModel>();
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync<ApiResponse<AppointmentViewModel>>("/Appointments", model);

            if (response != null && response.Success)
            {
                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = response?.Message ?? "Failed to create appointment";
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
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var response = await _apiService.PostAsync<ApiResponse<bool>>($"/Appointments/{id}/cancel", null);

            if (response != null && response.Success)
            {
                TempData["SuccessMessage"] = "Appointment cancelled successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel appointment";
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