using CinemaShowtimes.Infrastructure.Persistence;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaShowtimes.Infrastructure.Repositories;

public class MovieRepository(CinemaDbContext dbContext) : IMovieRepository
{
    public async Task AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await dbContext.Movies.AddAsync(movie, cancellationToken);
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Movies.AsNoTracking().ToListAsync(cancellationToken);
    }
}