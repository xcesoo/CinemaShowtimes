using Application.DTOs;
using MediatR;

namespace Application.Queries.Showtimes;

public readonly record struct GetShowtimeSeatMapQuery(Guid Id) : IRequest<ShowtimeSeatMapDto?>;