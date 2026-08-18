using Domain.Entities;
using MediatR;

namespace Application.Queries.Reservations;

public readonly record struct GetReservationByIdQuery(Guid Id) : IRequest<Reservation?>;