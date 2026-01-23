using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using LoginRequest = BookStore.Application.DTOs.LoginRequest;

namespace BookStore.Application.Services;

public class AuthService(IBookStoreDbContext context, IPasswordHasher hasher, IJwtTokenGenerator tokenGenerator)
    : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await ValidateUserCredentials(request.Email, request.Password);
        if (user == null)
            throw new Exception("Invalid Credentials");
        // 3. Generate Token
        var token = tokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, token);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // normalize data, always for critical stuff
        var normalizedEmail = request.Email.Trim().ToLower();
        // 1. Check if email exists
        if (await context.Users.AnyAsync(u => u.Email == normalizedEmail))
            throw new Exception("User already exists");

        // 2. Create User with Hashed Password
        var user = new User
        {
            Email = normalizedEmail,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = UserRole.Customer, // Default Role
            PasswordHash = hasher.Hash(request.Password)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(default);

        // 3. Auto-login after register (OR not depend on use case)
        var token = tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, token);
    }

    public async Task<User?> ValidateUserCredentials(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLower();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        if (user is null)
            return null;
        bool isValid = hasher.Verify(password, user.PasswordHash);
        return !isValid ? null : user;
    }
}