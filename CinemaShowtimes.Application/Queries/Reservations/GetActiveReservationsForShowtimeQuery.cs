using Domain.Entities;
using MediatR;

namespace Application.Queries.Reservations;

public readonly record struct GetActiveReservationsForShowtimeQuery(Guid ShowtimeId) : IRequest<IReadOnlyCollection<Reservation>>;