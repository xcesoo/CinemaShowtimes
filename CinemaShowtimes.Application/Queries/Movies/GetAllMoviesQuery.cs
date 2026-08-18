using Domain.Entities;
using MediatR;

namespace Application.Queries.Movies;

public readonly record struct GetAllMoviesQuery() : IRequest<IReadOnlyCollection<Movie>>;