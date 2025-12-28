using HMS.Web.Models;
using HMS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HMS.Web.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _apiService;

    public AuthController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // Check if already logged in
        var token = HttpContext.Session.GetString("JWTToken");
        if (!string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _apiService.PostAsync<ApiResponse<LoginResponse>>("/api/Auth/login", new
            {
                email = model.Email,
                password = model.Password
            });

            if (response != null && response.Success && response.Data != null)
            {
                // Save token in session
                HttpContext.Session.SetString("JWTToken", response.Data.AccessToken);
                HttpContext.Session.SetString("RefreshToken", response.Data.RefreshToken);
                HttpContext.Session.SetString("UserName", $"{response.Data.FirstName} {response.Data.LastName}");
                HttpContext.Session.SetString("UserEmail", response.Data.Email);
                HttpContext.Session.SetString("UserRole", response.Data.Role);
                HttpContext.Session.SetInt32("UserId", response.Data.UserId);

                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response?.Message ?? "Invalid login attempt");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(model);
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["SuccessMessage"] = "Logged out successfully!";
        return RedirectToAction("Login");
    }
}

public class LoginResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}