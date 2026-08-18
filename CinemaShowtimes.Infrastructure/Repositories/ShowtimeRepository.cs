using CinemaShowtimes.Infrastructure.Persistence;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaShowtimes.Infrastructure.Repositories;

public class ShowtimeRepository : IShowtimeRepository
{
    private readonly CinemaDbContext _dbContext;

    public ShowtimeRepository(CinemaDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Showtime showtime, CancellationToken cancellationToken = default)
    {
        await _dbContext.Showtimes.AddAsync(showtime, cancellationToken);
    }

    public async Task<Showtime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Showtimes.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}