using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Reservations;

public class ConfirmReservationCommandHandler(
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) 
    : IRequestHandler<ConfirmReservationCommand>
{
    public async Task Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        
        var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken)
                          ?? throw new KeyNotFoundException($"Reservation with ID {request.ReservationId} not found.");

        if (reservation.IsConfirmed)
            throw new DomainException("This reservation is already confirmed.");
        
        if (reservation.IsExpired(now))
        {
            throw new DomainException("Reservation has expired. You must complete the purchase within 10 minutes.");
        }
        
        reservation.Confirm(now);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}