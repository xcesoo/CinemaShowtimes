using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaShowtimes.Infrastructure.Persistence.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("movies");
        
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(m => m.Title)
            .HasMaxLength(100)
            .HasColumnName("title");
        
        builder.Property(m => m.Category)
            .HasMaxLength(100)
            .HasColumnName("category");

        builder.Property(m => m.Year)
            .HasColumnName("year");
    }
}