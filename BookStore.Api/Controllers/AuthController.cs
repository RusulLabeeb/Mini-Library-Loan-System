using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("/login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var result = await authService.LoginAsync(request);
            return Ok(new ApiResponse<AuthResponse>(result));
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse<AuthResponse>(new List<string> { e.Message }, "Login failed"));
        }
    }

    [HttpPost("/register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await authService.RegisterAsync(request);
            return Ok(new ApiResponse<AuthResponse>(result));
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse<AuthResponse>(new List<string> { e.Message }, "Registration failed"));
        }
    }
}