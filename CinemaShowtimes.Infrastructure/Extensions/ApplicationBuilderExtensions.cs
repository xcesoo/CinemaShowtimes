using CinemaShowtimes.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CinemaShowtimes.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            await DatabaseSeeder.SeedAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<CinemaDbContext>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}