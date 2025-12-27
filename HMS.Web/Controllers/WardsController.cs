using HMS.Web.Models;
using HMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers;

public class WardsController : Controller
{
    private readonly ApiService _apiService;

    public WardsController(ApiService apiService)
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
            var response = await _apiService.GetAsync<ApiResponse<List<WardViewModel>>>("/Wards");

            if (response != null && response.Success && response.Data != null)
            {
                return View(response.Data);
            }

            return View(new List<WardViewModel>());
        }
        catch (Exception)
        {
            return View(new List<WardViewModel>());
        }
    }
}