using MediatR;

namespace Application.Commands.Showtimes;

public readonly record struct CreateShowtimeCommand(Guid MovieId, Guid AuditoriumId,DateTime StartTime) : IRequest<Guid>;