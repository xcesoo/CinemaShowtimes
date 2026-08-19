using Application.DTOs;
using MediatR;

namespace Application.Commands.Reservations;

public readonly record struct ReserveContiguousSeatsCommand(Guid ShowtimeId, 
    short SeatCount) : IRequest<ReservationResultDto>;