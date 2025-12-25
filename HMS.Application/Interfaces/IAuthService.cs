using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.DTOs.Auth;
using HMS.Shared.Models;

namespace HMS.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ApiResponse<bool>> LogoutAsync(int userId);
}