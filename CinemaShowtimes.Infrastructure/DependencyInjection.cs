using CinemaShowtimes.Infrastructure.Persistence;
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
        return services;
    }
}