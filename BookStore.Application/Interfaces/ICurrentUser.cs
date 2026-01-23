using BookStore.Domain.Enums;

namespace BookStore.Application.Interfaces;

public interface ICurrentUser
{
    int Id { get; }
    UserRole Role { get; }
}