using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;

namespace CinemaShowtimes.Tests.Domain;

public class SeatTests
{
    /*
     * This test checks that the system does not allow creating a seat
     * with a row or number less than 1.
     */
    [Theory]
    [InlineData(0, 1)]   
    [InlineData(1, 0)] 
    [InlineData(-1, 5)]
    public void Create_WhenInvalidRowOrNumber_ShouldThrowDomainException(short row, short number)
    {
        // Act
        Action action = () => Seat.Create(row, number);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage($"Invalid seat R{row} N{number}");
    }

    /*
     * This test checks that a seat is created successfully
     * if the row and number are correct (greater than zero).
     */
    [Fact]
    public void Create_WhenValidRowAndNumber_ShouldCreateSeat()
    {
        // Act
        var seat = Seat.Create(1, 5);

        // Assert
        seat.Row.Should().Be(1);
        seat.Number.Should().Be(5);
    }
}