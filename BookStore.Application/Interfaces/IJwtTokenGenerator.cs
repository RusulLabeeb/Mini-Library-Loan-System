using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}