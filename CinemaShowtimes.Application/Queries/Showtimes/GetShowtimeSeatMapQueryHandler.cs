using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Showtimes;

public class GetShowtimeSeatMapQueryHandler(
    IShowtimeRepository showtimeRepository,
    IAuditoriumRepository auditoriumRepository,
    IReservationRepository reservationRepository) 
    : IRequestHandler<GetShowtimeSeatMapQuery, ShowtimeSeatMapDto?>
{
    public async Task<ShowtimeSeatMapDto?> Handle(GetShowtimeSeatMapQuery request, CancellationToken cancellationToken)
    {
        var showtime = await showtimeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (showtime is null) return null;

        var auditorium = await auditoriumRepository.GetByIdAsync(showtime.AuditoriumId, cancellationToken);
        if (auditorium is null) return null;
        
        var activeReservations = await reservationRepository
            .GetActiveReservationsForShowtimeAsync(request.Id, cancellationToken);

        var soldSeats = new HashSet<(short Row, short Number)>();
        var reservedSeats = new HashSet<(short Row, short Number)>();

        foreach (var res in activeReservations)
        {
            foreach (var seat in res.Seats)
            {
                if (res.IsConfirmed)
                    soldSeats.Add((seat.Row, seat.Number));
                else
                    reservedSeats.Add((seat.Row, seat.Number));
            }
        }

        var seatDtos = new List<SeatDto>();
        
        foreach (var physicalSeat in auditorium.Seats)
        {
            var seatTuple = (physicalSeat.Row, physicalSeat.Number);
            var status = SeatStatus.Available;

            if (soldSeats.Contains(seatTuple))
                status = SeatStatus.Sold;
            else if (reservedSeats.Contains(seatTuple))
                status = SeatStatus.Reserved;

            seatDtos.Add(new SeatDto(physicalSeat.Row, physicalSeat.Number, status));
        }

        return new ShowtimeSeatMapDto(showtime.Id, auditorium.Name, seatDtos);
    }
}