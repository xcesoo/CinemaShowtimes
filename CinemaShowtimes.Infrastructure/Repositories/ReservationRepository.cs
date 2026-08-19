using CinemaShowtimes.Infrastructure.Persistence;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaShowtimes.Infrastructure.Repositories;

public class ReservationRepository(CinemaDbContext dbContext) : IReservationRepository
{
    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await dbContext.Reservations.AddAsync(reservation, cancellationToken);
    }

    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservations
            .Include(r => r.Seats)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reservation>> GetActiveReservationsForShowtimeAsync(
        Guid showtimeId, 
        DateTimeOffset currentTime,
        CancellationToken cancellationToken = default)
    {
        var expirationTime = currentTime.AddMinutes(-10); 

        return await dbContext.Reservations
            .AsNoTracking()
            .Include(r => r.Seats)
            .Where(r => r.ShowtimeId == showtimeId && (r.IsConfirmed || r.CreatedAt > expirationTime))
            .ToListAsync(cancellationToken);
    }
}