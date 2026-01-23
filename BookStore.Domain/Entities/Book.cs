using System.ComponentModel.DataAnnotations;
using BookStore.Domain.Entities.Common;
using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Entities;

public class Book : BaseEntity, IAuditable, ISoftDeletable
{
    [MaxLength(200)]
    public string Title { get; set; }
    
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    
    public DateTime? PublishYear { get; set; }
    
    public ICollection<BookGenre> Genres { get; set; }

    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}