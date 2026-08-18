using Domain.Entities;

namespace Domain.Interfaces;

public interface IMovieRepository
{
    Task AddAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}