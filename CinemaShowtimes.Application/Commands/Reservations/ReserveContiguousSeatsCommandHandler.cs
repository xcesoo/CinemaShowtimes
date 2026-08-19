using System.Data;
using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.Reservations;

public class ReserveContiguousSeatsCommandHandler(
    IShowtimeRepository showtimeRepository,
    IAuditoriumRepository auditoriumRepository,
    IReservationRepository reservationRepository,
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<ReserveContiguousSeatsCommand, ReservationResultDto>
{
    public async Task<ReservationResultDto> Handle(ReserveContiguousSeatsCommand request, CancellationToken cancellationToken)
    {
        if (request.SeatCount <= 0)
            throw new DomainException("Seat count must be greater than zero.");

        await using var transaction = await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            var showtime = await showtimeRepository.GetByIdAsync(request.ShowtimeId, cancellationToken)
                           ?? throw new DomainException($"Showtime with ID {request.ShowtimeId} not found.");

            var auditorium = await auditoriumRepository.GetByIdAsync(showtime.AuditoriumId, cancellationToken)
                             ?? throw new DomainException("Auditorium not found.");

            var movie = await movieRepository.GetByIdAsync(showtime.MovieId, cancellationToken);

            var activeReservations = await reservationRepository
                .GetActiveReservationsForShowtimeAsync(request.ShowtimeId, cancellationToken);

            var takenSeats = activeReservations.SelectMany(r => r.Seats).ToHashSet();

            var availableSeats = auditorium.Seats.Except(takenSeats).ToList();

            List<Seat>? contiguousSeats = null;

            foreach (var rowGroup in availableSeats.GroupBy(s => s.Row).OrderBy(g => g.Key))
            {
                var rowSeats = rowGroup.OrderBy(s => s.Number).ToList();
                
                if (rowSeats.Count < request.SeatCount)
                    continue;

                for (int i = 0; i <= rowSeats.Count - request.SeatCount; i++)
                {
                    bool isContiguous = true;

                    for (int j = 0; j < request.SeatCount - 1; j++)
                    {
                        if (rowSeats[i + j + 1].Number != rowSeats[i + j].Number + 1)
                        {
                            isContiguous = false;
                            break;
                        }
                    }

                    if (isContiguous)
                    {
                        contiguousSeats = rowSeats.GetRange(i, request.SeatCount);
                        break;
                    }
                }

                if (contiguousSeats is not null)
                    break;
            }

            if (contiguousSeats is null)
            {
                throw new DomainException($"Could not find {request.SeatCount} contiguous seats together for this showtime.");
            }

            var reservation = Reservation.Create(request.ShowtimeId, contiguousSeats);

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