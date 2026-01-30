using System.ComponentModel.DataAnnotations;
using BookStore.Domain.Entities.Common;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Entities;

public class User : BaseEntity, IAuditable, ISoftDeletable
{
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    [MaxLength(45)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(45)]
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;

    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Loan> Loans { get; set; }
}