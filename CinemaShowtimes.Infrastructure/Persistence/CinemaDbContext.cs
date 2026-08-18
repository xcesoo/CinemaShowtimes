using CinemaShowtimes.Infrastructure.Persistence.Configurations;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CinemaShowtimes.Infrastructure.Persistence;

public class CinemaDbContext : DbContext
{
    public DbSet<Movie>  Movies { get; set; }
    public DbSet<Auditorium> Auditoriums { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Showtime>  Showtimes { get; set; }
    public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieConfiguration).Assembly); 
    }
}