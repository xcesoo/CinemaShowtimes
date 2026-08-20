using System.Data;
using Application.Commands.Reservations;
using CinemaShowtimes.Tests.Helpers;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace CinemaShowtimes.Tests.Application.Commands;

public class ReserveContiguousSeatsCommandHandlerTests
{
    private readonly Mock<IShowtimeRepository> _showtimeRepoMock = new();
    private readonly Mock<IAuditoriumRepository> _auditoriumRepoMock = new();
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IMovieRepository> _movieRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly FakeTimeProvider _timeProvider = new();

    private readonly ReserveContiguousSeatsCommandHandler _handler;
 
    public ReserveContiguousSeatsCommandHandlerTests()
    {
        _uowMock.Setup(u => u.BeginTransactionAsync(It.IsAny<IsolationLevel>(), default))
            .ReturnsAsync(new DummyTransaction());

        _handler = new ReserveContiguousSeatsCommandHandler(
            _showtimeRepoMock.Object,
            _auditoriumRepoMock.Object,
            _reservationRepoMock.Object,
            _movieRepoMock.Object,
            _uowMock.Object,
            _timeProvider);
    }

    /*
     * This test checks if the algorithm successfully finds
     * and reserves seats that are next to each other (contiguous)
     * when there are available spaces.
     */
    [Fact]
    public async Task Handle_WhenContiguousSeatsAvailable_ShouldReserveThem()
    {
        // Arrange
        var showtimeId = Guid.NewGuid();
        var auditoriumId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        
        var seats = new List<Seat> { 
            Seat.Create(1, 1), Seat.Create(1, 2), Seat.Create(1, 3), Seat.Create(1, 4), Seat.Create(1, 5) 
        };
        var auditorium = Auditorium.Create("Test Hall", seats);
        typeof(Auditorium).GetProperty("Id")!.SetValue(auditorium, auditoriumId);

        var showtime = Showtime.Create(movieId, auditoriumId, DateTimeOffset.UtcNow);
        typeof(Showtime).GetProperty("Id")!.SetValue(showtime, showtimeId);

        var movie = Movie.Create("Test Movie", "Action", 2024);
        
        var takenSeats = new List<Seat> { Seat.Create(1, 1), Seat.Create(1, 4) };
        var existingReservation = Reservation.Create(showtimeId, takenSeats, _timeProvider.GetUtcNow());

        _showtimeRepoMock.Setup(r => r.GetByIdAsync(showtimeId, default)).ReturnsAsync(showtime);
        _auditoriumRepoMock.Setup(r => r.GetByIdAsync(auditoriumId, default)).ReturnsAsync(auditorium);
        _movieRepoMock.Setup(r => r.GetByIdAsync(movieId, default)).ReturnsAsync(movie);
        
        _reservationRepoMock.Setup(r => r.GetActiveReservationsForShowtimeAsync(showtimeId, _timeProvider.GetUtcNow(), default))
            .ReturnsAsync(new List<Reservation> { existingReservation });

        var command = new ReserveContiguousSeatsCommand(showtimeId, SeatCount: 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.NumberOfSeats.Should().Be(2);
        
        _reservationRepoMock.Verify(r => r.AddAsync(It.Is<Reservation>(res => 
            res.Seats.Count == 2 &&
            res.Seats.Any(s => s.Row == 1 && s.Number == 2) &&
            res.Seats.Any(s => s.Row == 1 && s.Number == 3)
        ), default), Times.Once);

        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    
    /*
     * This test checks if the system throws an error
     * when a user asks for contiguous seats, but the available seats
     * are separated by other reserved seats (no continuous block found).
     */
    [Fact]
    public async Task Handle_WhenNotEnoughContiguousSeats_ShouldThrowDomainException()
    {
        // Arrange 
        var showtimeId = Guid.NewGuid();
        var auditoriumId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        
        var seats = new List<Seat> { Seat.Create(1, 1), Seat.Create(1, 2), Seat.Create(1, 3) };
        var auditorium = Auditorium.Create("Test Hall", seats);
        typeof(Auditorium).GetProperty("Id")!.SetValue(auditorium, auditoriumId);

        var showtime = Showtime.Create(movieId, auditoriumId, DateTimeOffset.UtcNow);
        typeof(Showtime).GetProperty("Id")!.SetValue(showtime, showtimeId);

        var takenSeats = new List<Seat> { Seat.Create(1, 2) };
        var existingReservation = Reservation.Create(showtimeId, takenSeats, _timeProvider.GetUtcNow());

        _showtimeRepoMock.Setup(r => r.GetByIdAsync(showtimeId, default)).ReturnsAsync(showtime);
        _auditoriumRepoMock.Setup(r => r.GetByIdAsync(auditoriumId, default)).ReturnsAsync(auditorium);
        _reservationRepoMock.Setup(r => r.GetActiveReservationsForShowtimeAsync(showtimeId, _timeProvider.GetUtcNow(), default))
            .ReturnsAsync(new List<Reservation> { existingReservation });

        var command = new ReserveContiguousSeatsCommand(showtimeId, SeatCount: 2);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("*Could not find 2 contiguous seats*");
    }
}