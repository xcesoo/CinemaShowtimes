using Application.DTOs;
using MediatR;

namespace Application.Commands.Reservations;

public readonly record struct ReserveSeatsCommand(
    Guid ShowtimeId, 
    IReadOnlyCollection<ReserveSeatRequest> Seats) : IRequest<ReservationResultDto>;