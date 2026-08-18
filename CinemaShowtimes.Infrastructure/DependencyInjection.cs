using CinemaShowtimes.Infrastructure.Persistence;
using CinemaShowtimes.Infrastructure.Repositories;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaShowtimes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CinemaDbContext>(o =>
        {
            o.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<CinemaDbContext>());

        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IAuditoriumRepository, AuditoriumRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
        
        return services;
    }
}