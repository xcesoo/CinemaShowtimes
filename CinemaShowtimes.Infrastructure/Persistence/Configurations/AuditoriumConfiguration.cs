using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaShowtimes.Infrastructure.Persistence.Configurations;

public class AuditoriumConfiguration : IEntityTypeConfiguration<Auditorium>
{
    public void Configure(EntityTypeBuilder<Auditorium> builder)
    {
        builder.ToTable("auditoriums");
        
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(a => a.Name)
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.OwnsMany(a => a.Seats, seatBuilder =>
        {
            seatBuilder.ToTable("auditorium_seats");
            
            seatBuilder.Property(s => s.Row)
                .HasColumnName("row");
                
            seatBuilder.Property(s => s.Number)
                .HasColumnName("number");
        });
    }
}