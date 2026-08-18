using Domain.Entities;

namespace Domain.Interfaces;

public interface IShowtimeRepository
{
    Task AddAsync(Showtime showtime, CancellationToken cancellationToken = default);
    Task<Showtime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}