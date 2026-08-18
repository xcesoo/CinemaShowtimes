using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reservations;

public class GetActiveReservationsForShowtimeQueryHandler(IReservationRepository reservationRepository)
    : IRequestHandler<GetActiveReservationsForShowtimeQuery, IReadOnlyCollection<Reservation>>
{
    public async Task<IReadOnlyCollection<Reservation>> Handle(GetActiveReservationsForShowtimeQuery request, CancellationToken cancellationToken)
    {
        return await reservationRepository.GetActiveReservationsForShowtimeAsync(request.ShowtimeId, cancellationToken);
    }
}