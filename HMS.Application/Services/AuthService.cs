using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Auth;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using HMS.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Invalid email or password");
            }

            if (!user.IsActive)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Account is deactivated");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse($"Login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Email already exists");
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Address = request.Address,
                Role = request.Role,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Registration successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Invalid access token");
            }

            var userId = int.Parse(principal.FindFirst("UserId")!.Value);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ApiResponse<LoginResponseDto>.FailureResponse("Invalid refresh token");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.FailureResponse($"Token refresh failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> LogoutAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<bool>.FailureResponse("User not found");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Logout successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Logout failed: {ex.Message}");
        }
    }
}