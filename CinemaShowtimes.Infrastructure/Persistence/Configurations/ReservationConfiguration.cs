using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaShowtimes.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(r => r.ShowtimeId)
            .HasColumnName("showtime_id");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(r => r.IsConfirmed)
            .HasColumnName("is_confirmed");

        builder.HasOne<Showtime>()
            .WithMany()
            .HasForeignKey(r => r.ShowtimeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(r => r.Seats, seatBuilder =>
        {
            seatBuilder.ToTable("reservation_seats");
            
            seatBuilder.Property(s => s.Row)
                .HasColumnName("row");
                
            seatBuilder.Property(s => s.Number)
                .HasColumnName("number");
        });
    }
}