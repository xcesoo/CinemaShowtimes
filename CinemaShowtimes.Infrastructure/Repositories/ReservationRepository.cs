using CinemaShowtimes.Infrastructure.Persistence;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaShowtimes.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly CinemaDbContext _dbContext;

    public ReservationRepository(CinemaDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
    }

    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .Include(r => r.Seats)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reservation>> GetActiveReservationsForShowtimeAsync(
        Guid showtimeId, 
        CancellationToken cancellationToken = default)
    {
        var expirationTime = DateTimeOffset.UtcNow.AddMinutes(-10);

        return await _dbContext.Reservations
            .AsNoTracking()
            .Include(r => r.Seats)
            .Where(r => r.ShowtimeId == showtimeId && (r.IsConfirmed || r.CreatedAt > expirationTime))
            .ToListAsync(cancellationToken);
    }
}