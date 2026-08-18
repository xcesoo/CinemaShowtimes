using Domain.Entities;
using MediatR;

namespace Application.Queries.Showtimes;

public readonly record struct GetAllShowtimesQuery() : IRequest<IReadOnlyCollection<Showtime>>;