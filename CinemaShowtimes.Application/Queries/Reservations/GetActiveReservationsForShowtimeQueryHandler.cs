using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reservations;

public class GetActiveReservationsForShowtimeQueryHandler(IReservationRepository reservationRepository, TimeProvider timeProvider)
    : IRequestHandler<GetActiveReservationsForShowtimeQuery, IReadOnlyCollection<Reservation>>
{
    public async Task<IReadOnlyCollection<Reservation>> Handle(GetActiveReservationsForShowtimeQuery request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        return await reservationRepository.GetActiveReservationsForShowtimeAsync(request.ShowtimeId, now, cancellationToken);
    }
}