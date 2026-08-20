using System.Data;
using Application.Commands.Reservations;
using Application.DTOs;
using CinemaShowtimes.Tests.Helpers;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace CinemaShowtimes.Tests.Application.Commands;

public class ReserveSeatsCommandHandlerTests
{
    private readonly Mock<IShowtimeRepository> _showtimeRepoMock = new();
    private readonly Mock<IAuditoriumRepository> _auditoriumRepoMock = new();
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IMovieRepository> _movieRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly ReserveSeatsCommandHandler _handler;

    public ReserveSeatsCommandHandlerTests()
    {
        _uowMock.Setup(u => u.BeginTransactionAsync(It.IsAny<IsolationLevel>(), default))
            .ReturnsAsync(new DummyTransaction());

        _handler = new ReserveSeatsCommandHandler(
            _showtimeRepoMock.Object, _auditoriumRepoMock.Object, 
            _reservationRepoMock.Object, _movieRepoMock.Object, 
            _uowMock.Object, _timeProvider);
    }

    
    /*
     * This test checks if the system throws an error
     * when a user tries to reserve a seat that is already taken by someone else.
     */
    [Fact]
    public async Task Handle_WhenSeatAlreadyReserved_ShouldThrowDomainException()
    {
        // Arrange
        var showtimeId = Guid.NewGuid();
        var auditoriumId = Guid.NewGuid();
        
        var seats = new List<Seat> { Seat.Create(1, 1), Seat.Create(1, 2) };
        var auditorium = Auditorium.Create("Test Hall", seats);
        var showtime = Showtime.Create(Guid.NewGuid(), auditoriumId, DateTimeOffset.UtcNow);

        var existingReservation = Reservation.Create(showtimeId, new[] { Seat.Create(1, 1) }, _timeProvider.GetUtcNow());

        _showtimeRepoMock.Setup(r => r.GetByIdAsync(showtimeId, default)).ReturnsAsync(showtime);
        _auditoriumRepoMock.Setup(r => r.GetByIdAsync(showtime.AuditoriumId, default)).ReturnsAsync(auditorium);
        _reservationRepoMock.Setup(r => r.GetActiveReservationsForShowtimeAsync(showtimeId, _timeProvider.GetUtcNow(), default))
            .ReturnsAsync(new List<Reservation> { existingReservation });
        
        var command = new ReserveSeatsCommand(showtimeId, new[] { new ReserveSeatRequest(1, 1) });

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("*already reserved or sold*");
    }
    
    /*
     * This test checks if the system successfully creates a new reservation
     * when the requested seats are completely free.
     */
    [Fact]
    public async Task Handle_WhenSeatsAreAvailable_ShouldCreateReservation()
    {
        // Arrange
        var showtimeId = Guid.NewGuid();
        var auditoriumId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        
        var auditorium = Auditorium.Create("Test Hall", new[] { Seat.Create(1, 1), Seat.Create(1, 2) });
        var showtime = Showtime.Create(movieId, auditoriumId, DateTimeOffset.UtcNow);
        typeof(Showtime).GetProperty("Id")!.SetValue(showtime, showtimeId); // Хакаємо ID
        var movie = Movie.Create("Test Movie", "Action", 2026);

        _showtimeRepoMock.Setup(r => r.GetByIdAsync(showtimeId, default)).ReturnsAsync(showtime);
        _auditoriumRepoMock.Setup(r => r.GetByIdAsync(auditoriumId, default)).ReturnsAsync(auditorium);
        _movieRepoMock.Setup(r => r.GetByIdAsync(movieId, default)).ReturnsAsync(movie);
        
        _reservationRepoMock.Setup(r => r.GetActiveReservationsForShowtimeAsync(showtimeId, It.IsAny<DateTimeOffset>(), default))
            .ReturnsAsync(new List<Reservation>());

        var command = new ReserveSeatsCommand(showtimeId, new[] { new ReserveSeatRequest(1, 1), new ReserveSeatRequest(1, 2) });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.NumberOfSeats.Should().Be(2);
        
        _reservationRepoMock.Verify(r => r.AddAsync(It.IsAny<Reservation>(), default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}