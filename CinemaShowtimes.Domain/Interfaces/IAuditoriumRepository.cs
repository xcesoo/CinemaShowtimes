using Domain.Entities;

namespace Domain.Interfaces;

public interface IAuditoriumRepository
{
    Task<Auditorium?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}