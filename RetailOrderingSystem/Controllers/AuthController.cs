/*
 * Folder: Controllers
 * File: AuthController.cs
 * Purpose: HTTP entry point for authentication — register, login, refresh token.
 * Who Calls It: HTTP clients (Postman, frontend apps)
 * Flow: HTTP Request → AuthController → IAuthService → AppDbContext → SQL Server
 * Interview Tip: Controllers are thin — they only parse requests and return responses.
 *                All business logic lives in the service layer.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.DTOs.Auth;
using RetailOrderingSystem.Interfaces;
using System.Security.Claims;

namespace RetailOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Register a new customer account</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return StatusCode(201, result);
        }

        /// <summary>Login and receive JWT + RefreshToken</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        /// <summary>Exchange a refresh token for a new access token</summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(result);
        }

        /// <summary>Get current user's profile</summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.GetProfileAsync(userId);
            return Ok(result);
        }

        /// <summary>Update current user's profile</summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.UpdateProfileAsync(userId, request);
            return Ok(result);
        }

        /// <summary>Change current user's password</summary>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _authService.ChangePasswordAsync(userId, request);
            return Ok(new { Message = "Password changed successfully." });
        }

        /// <summary>Get all customers (Admin only)</summary>
        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _authService.GetAllCustomersAsync();
            return Ok(result);
        }
    }
}
