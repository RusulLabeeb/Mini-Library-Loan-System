using System.Security.Claims;
using BookStore.Application.Interfaces;
using BookStore.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace BookStore.Infrastructure.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public int Id
    {
        get
        {
            // We search for the "sub" claim we added in JwtTokenGenerator
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return 0;
            return int.Parse(claim.Value);
        }
    }

    public UserRole Role
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
            if (claim == null) return UserRole.Customer; // Default fall back or we can throw an exception
            // Parse string "Admin" back to Enum UserRole.Admin
            return Enum.Parse<UserRole>(claim.Value);
        }
    }
}