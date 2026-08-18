using CinemaShowtimes.Infrastructure.Persistence;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaShowtimes.Infrastructure.Repositories;

public class AuditoriumRepository : IAuditoriumRepository
{
    private readonly CinemaDbContext _dbContext;

    public AuditoriumRepository(CinemaDbContext dbContext) => _dbContext = dbContext;

    public async Task<Auditorium?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Auditoriums
            .AsNoTracking()
            .Include(a => a.Seats)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}