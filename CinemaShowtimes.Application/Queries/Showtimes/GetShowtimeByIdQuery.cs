using Domain.Entities;
using MediatR;

namespace Application.Queries.Showtimes;

public readonly record struct GetShowtimeByIdQuery(Guid Id) : IRequest<Showtime?>;