using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaShowtimes.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();

        await context.Database.MigrateAsync();

        if (await context.Auditoriums.AnyAsync())
        {
            return;
        }

        var seats = new List<Seat>();
        for (short row = 1; row <= 10; row++)
        {
            for (short number = 1; number <= 15; number++)
            {
                seats.Add(Seat.Create(row, number));
            }
        }
        var auditorium = Auditorium.Create("IMAX Hall 1", seats);
        context.Auditoriums.Add(auditorium);

        var movies = new List<Movie>
        {
            Movie.Create("Inception", "Sci-Fi", 2010),
            Movie.Create("The Dark Knight", "Action", 2008),
            Movie.Create("Interstellar", "Sci-Fi", 2014)
        };
        context.Movies.AddRange(movies);

        var showtime = Showtime.Create(
            movies.First().Id, 
            auditorium.Id, 
            DateTimeOffset.UtcNow.AddHours(2)); 
            
        context.Showtimes.Add(showtime);

        await context.SaveChangesAsync();
    }
}