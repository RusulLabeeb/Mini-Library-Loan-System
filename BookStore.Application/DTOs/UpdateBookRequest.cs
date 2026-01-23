using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.DTOs;

public class UpdateBookRequest
{
    [Required]
    public int Id { get; set; }
    public string? Title { get; set; }
    public int? AuthorId { get; set; }
}