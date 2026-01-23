using System.Linq.Expressions;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistance;

public class BookStoreDbContext : DbContext, IBookStoreDbContext
{
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
    {
    }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<AuthorBio> AuthorBios { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog>  AuditLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookStoreDbContext).Assembly);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)) continue;
            modelBuilder.Entity(entityType.ClrType)
                .Property("IsDeleted")
                .HasDefaultValue(false);
            
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var falseConstant = Expression.Constant(false); // Handle nullable if needed
            var binaryExpression = Expression.Equal(property, falseConstant);
            var filter = Expression.Lambda(binaryExpression, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            
            foreach (var index in entityType.GetIndexes())
            {
                // Safety: If I manually defined a complex SQL filter in our configuration class, don't overwrite it
                if (string.IsNullOrEmpty(index.GetFilter())) 
                {
                    index.SetFilter($"\"IsDeleted\" = false"); 
                }
            }
        }
    }
    
    
}