using MediatR;

namespace Application.Commands.Reservations;

public readonly record struct ConfirmReservationCommand(Guid ReservationId) : IRequest;