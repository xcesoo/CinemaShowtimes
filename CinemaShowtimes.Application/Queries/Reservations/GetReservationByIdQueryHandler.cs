using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reservations;

public class GetReservationByIdQueryHandler(IReservationRepository reservationRepository)
    : IRequestHandler<GetReservationByIdQuery, Reservation?>
{
    public async Task<Reservation?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        return await reservationRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}