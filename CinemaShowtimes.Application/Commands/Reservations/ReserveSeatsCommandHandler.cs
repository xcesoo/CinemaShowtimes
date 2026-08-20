using System.Data;
using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.Reservations;

public class ReserveSeatsCommandHandler(
    IShowtimeRepository showtimeRepository,
    IAuditoriumRepository auditoriumRepository,
    IReservationRepository reservationRepository,
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) 
    : IRequestHandler<ReserveSeatsCommand, ReservationResultDto>
{
    public async Task<ReservationResultDto> Handle(ReserveSeatsCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        
        if (request.Seats is null || request.Seats.Count == 0)
            throw new DomainException("At least one seat must be selected.");
        
        if (request.Seats.Count != request.Seats.Distinct().Count())
            throw new DomainException("Duplicate seats are not allowed.");
            
        await using var transaction = await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {

            var showtime = await showtimeRepository.GetByIdAsync(request.ShowtimeId, cancellationToken)
                           ?? throw new DomainException($"Showtime with ID {request.ShowtimeId} not found.");

            var auditorium = await auditoriumRepository.GetByIdAsync(showtime.AuditoriumId, cancellationToken)
                             ?? throw new DomainException("Auditorium not found.");

            var movie = await movieRepository.GetByIdAsync(showtime.MovieId, cancellationToken);

            var requestedSeats = request.Seats
                .Select(s => Seat.Create(s.Row, s.Number))
                .ToList();

            var invalidSeats = requestedSeats.Except(auditorium.Seats).ToList();
            if (invalidSeats.Count > 0)
            {
                var invalidSeatsStr = string.Join(", ", invalidSeats.Select(s => $"R{s.Row} N{s.Number}"));
                throw new DomainException($"The following seats do not exist in the auditorium: {invalidSeatsStr}");
            }

            var activeReservations = await reservationRepository
                .GetActiveReservationsForShowtimeAsync(request.ShowtimeId, now, cancellationToken);

            var takenSeats = activeReservations.SelectMany(r => r.Seats).ToHashSet();

            var overlappingSeats = requestedSeats.Intersect(takenSeats).ToList();
            if (overlappingSeats.Count > 0)
            {
                var overlappingSeatsStr = string.Join(", ", overlappingSeats.Select(s => $"R{s.Row} N{s.Number}"));
                throw new DomainException($"The following seats are already reserved or sold: {overlappingSeatsStr}");
            }

            var reservation = Reservation.Create(request.ShowtimeId, requestedSeats, now);

            await reservationRepository.AddAsync(reservation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            return new ReservationResultDto(
                reservation.Id,
                reservation.Seats.Count,
                auditorium.Name,
                movie!.Title
            );
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}