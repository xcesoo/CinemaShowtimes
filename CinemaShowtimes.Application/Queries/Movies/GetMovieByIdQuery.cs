using Domain.Entities;
using MediatR;

namespace Application.Queries.Movies;

public readonly record struct GetMovieByIdQuery(Guid Id) : IRequest<Movie?>;