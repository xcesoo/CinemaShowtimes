using Application.DTOs;
using Application.Queries.Showtimes;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace CinemaShowtimes.Tests.Application.Queries;

public class GetShowtimeSeatMapQueryHandlerTests
{
    /*
     * This test checks if the seat map returns the correct status for each seat:
     * - Sold (if the reservation is confirmed)
     * - Reserved (if the reservation is not confirmed yet)
     * - Available (if there is no reservation)
     */
    [Fact]
    public async Task Handle_ShouldMapSeatStatusesCorrectly()
    {
        // Arrange
        var showtimeId = Guid.NewGuid();
        var auditoriumId = Guid.NewGuid();
        var timeProvider = new FakeTimeProvider();
        
        var auditorium = Auditorium.Create("Test Hall", new[] { Seat.Create(1, 1), Seat.Create(1, 2), Seat.Create(1, 3) });
        var showtime = Showtime.Create(Guid.NewGuid(), auditoriumId, DateTimeOffset.UtcNow);

        var confirmedRes = Reservation.Create(showtimeId, new[] { Seat.Create(1, 1) }, timeProvider.GetUtcNow());
        confirmedRes.Confirm(timeProvider.GetUtcNow());

        var unconfirmedRes = Reservation.Create(showtimeId, new[] { Seat.Create(1, 2) }, timeProvider.GetUtcNow());

        var showtimeRepoMock = new Mock<IShowtimeRepository>();
        showtimeRepoMock.Setup(r => r.GetByIdAsync(showtimeId, default)).ReturnsAsync(showtime);

        var auditoriumRepoMock = new Mock<IAuditoriumRepository>();
        auditoriumRepoMock.Setup(r => r.GetByIdAsync(auditoriumId, default)).ReturnsAsync(auditorium);

        var reservationRepoMock = new Mock<IReservationRepository>();
        reservationRepoMock.Setup(r => r.GetActiveReservationsForShowtimeAsync(showtimeId, It.IsAny<DateTimeOffset>(), default))
            .ReturnsAsync(new List<Reservation> { confirmedRes, unconfirmedRes });

        var handler = new GetShowtimeSeatMapQueryHandler(
            showtimeRepoMock.Object, auditoriumRepoMock.Object, reservationRepoMock.Object, timeProvider);

        // Act
        var result = await handler.Handle(new GetShowtimeSeatMapQuery(showtimeId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Seats.Should().HaveCount(3);
        
        result.Seats.Single(s => s.Number == 1).Status.Should().Be(SeatStatus.Sold);
        result.Seats.Single(s => s.Number == 2).Status.Should().Be(SeatStatus.Reserved);
        result.Seats.Single(s => s.Number == 3).Status.Should().Be(SeatStatus.Available);
    }
}