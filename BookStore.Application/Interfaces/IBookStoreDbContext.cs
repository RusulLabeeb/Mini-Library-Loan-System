using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Interfaces;

public interface IBookStoreDbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<AuthorBio> AuthorBios { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog>  AuditLogs { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    int SaveChanges();
}