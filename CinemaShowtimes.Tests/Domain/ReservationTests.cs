using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace CinemaShowtimes.Tests.Domain;

public class ReservationTests
{
    private readonly Guid _showtimeId = Guid.NewGuid();
    private readonly List<Seat> _seats = [Seat.Create(1, 1), Seat.Create(1, 2)];
    private readonly FakeTimeProvider _timeProvider = new();

    /*
     * This test checks if the reservation becomes expired
     * after exactly 10 minutes have passed.
     */
    [Fact]
    public void IsExpired_WhenExactly10MinutesPassed_ShouldBeTrue()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        _timeProvider.SetUtcNow(now);
        var reservation = Reservation.Create(_showtimeId, _seats, _timeProvider.GetUtcNow());

        // Act
        _timeProvider.Advance(TimeSpan.FromMinutes(10));
        var isExpired = reservation.IsExpired(_timeProvider.GetUtcNow());

        // Assert
        isExpired.Should().BeTrue();
    }
    
    /*
     * This test checks if the system throws an error
     * when a user tries to confirm a ticket that is already expired.
     */
    
    [Fact]
    public void Confirm_WhenAlreadyExpired_ShouldThrowDomainException()
    {
        // Arrange
        var reservation = Reservation.Create(_showtimeId, _seats, _timeProvider.GetUtcNow());
        _timeProvider.Advance(TimeSpan.FromMinutes(11)); // Протухло

        // Act
        Action action = () => reservation.Confirm(_timeProvider.GetUtcNow());

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Reservation has expired and cannot be confirmed.");
    }
}