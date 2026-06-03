/*
 * Folder: Services
 * File: AuthService.cs
 * Purpose: Handles all authentication business logic — register, login, refresh token.
 * Who Calls It: AuthController
 * Flow: Controller → AuthService → AppDbContext → SQL Server
 * Interview Tip: Services contain business logic. Controllers only handle HTTP.
 *                BCrypt hashes passwords. JWT tokens carry user identity.
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Constants;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Auth;
using RetailOrderingSystem.Helpers;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, JwtHelper jwtHelper,
            IEmailService emailService, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
                throw new InvalidOperationException("Email already registered.");

            // Create new user with hashed password
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.ToLower(),
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Role = AppConstants.RoleCustomer
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create loyalty points record for new customer
            _context.LoyaltyPoints.Add(new LoyaltyPoint { UserId = user.Id, Points = 0 });
            await _context.SaveChangesAsync();

            // Send welcome email (fire and forget — don't fail registration if email fails)
            _ = Task.Run(async () =>
            {
                try { await _emailService.SendRegistrationEmailAsync(user.Email, user.FirstName); }
                catch (Exception ex) { _logger.LogWarning("Email failed: {msg}", ex.Message); }
            });

            _logger.LogInformation("New customer registered: {email}", user.Email);

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && !u.IsDeleted);

            // Generic error — don't reveal which field is wrong (security best practice)
            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            _logger.LogInformation("User logged in: {email}", user.Email);
            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken
                    && u.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            return MapToProfileDto(user);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToProfileDto(user);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            if (!PasswordHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
            user.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == AppConstants.RoleCustomer && !u.IsDeleted)
                .Select(u => MapToProfileDto(u))
                .ToListAsync();
        }

        // Private helper — generates JWT + RefreshToken and saves to DB
        private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
        {
            var accessToken = _jwtHelper.GenerateAccessToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = user.Role,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                UserId = user.Id
            };
        }

        private static UserProfileDto MapToProfileDto(User user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Role = user.Role,
            CreatedDate = user.CreatedDate
        };
    }
}
