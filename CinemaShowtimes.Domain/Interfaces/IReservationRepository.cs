using Domain.Entities;

namespace Domain.Interfaces;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Reservation>> GetActiveReservationsForShowtimeAsync(
        Guid showtimeId, 
        DateTimeOffset currentTime,
        CancellationToken cancellationToken = default);
}