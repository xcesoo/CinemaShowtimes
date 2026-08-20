using Application.Commands.Reservations;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace CinemaShowtimes.Tests.Application.Commands;

public class ConfirmReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly ConfirmReservationCommandHandler _handler;

    public ConfirmReservationCommandHandlerTests()
    {
        _handler = new ConfirmReservationCommandHandler(
            _reservationRepoMock.Object, _uowMock.Object, _timeProvider);
    }

    /*
     * This test checks if the system throws an error
     * when the user tries to buy (confirm) a ticket after the 10-minute limit.
     */
    [Fact]
    public async Task Handle_WhenReservationIsExpired_ShouldThrowDomainException()
    {
        // Arrange
        var reservationId = Guid.NewGuid();
        var seats = new List<Seat> { Seat.Create(1, 1) };
        
        var reservation = Reservation.Create(Guid.NewGuid(), seats, _timeProvider.GetUtcNow());
        
        _reservationRepoMock.Setup(r => r.GetByIdAsync(reservationId, default)).ReturnsAsync(reservation);

        _timeProvider.Advance(TimeSpan.FromMinutes(11));

        var command = new ConfirmReservationCommand(reservationId);

        // Act
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("Reservation has expired. You must complete the purchase within 10 minutes.");
    }
}