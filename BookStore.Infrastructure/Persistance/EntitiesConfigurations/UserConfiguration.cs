using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistance.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
        // Indexing Email is crucial for Login performance
        builder.HasIndex(u => u.Email).IsUnique();

        // Enum Conversion: C# Enum <-> DB String
        builder.Property(u => u.Role)
            .HasConversion<string>(); 
    }
}