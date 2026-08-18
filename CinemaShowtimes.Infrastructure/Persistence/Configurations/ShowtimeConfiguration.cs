using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaShowtimes.Infrastructure.Persistence.Configurations;

public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
{
    public void Configure(EntityTypeBuilder<Showtime> builder)
    {
        builder.ToTable("showtimes");
        
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(s => s.MovieId)
            .HasColumnName("movie_id");

        builder.Property(s => s.AuditoriumId)
            .HasColumnName("auditorium_id");

        builder.Property(s => s.StartTime)
            .HasColumnName("start_time");

        builder.HasOne<Movie>()
            .WithMany()
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Auditorium>()
            .WithMany()
            .HasForeignKey(s => s.AuditoriumId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}