using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Reservation
{
    public Guid Id { get; init; } 
    public Guid ShowtimeId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsConfirmed { get; private set; } 

    private readonly List<Seat> _seats = new();
    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();
    
    private Reservation(){} //for ef core

    public static Reservation Create(Guid showtimeId, IEnumerable<Seat> seats, DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(seats);
        var seatList = seats.ToList();
        if (seatList.Count == 0) 
            throw new ArgumentException("Reservation must contain at least one seat.", nameof(seats));
        
        if (seatList.Count != seatList.Distinct().Count())
            throw new DomainException("Reservation cannot contain duplicate seats.");
        
        var reservation = new Reservation()
        {
            Id = Guid.CreateVersion7(),
            ShowtimeId = showtimeId,
            CreatedAt = createdAt,
            IsConfirmed = false,
        };
        reservation._seats.AddRange(seatList);
        
        return reservation;
    }
    public bool IsExpired(DateTimeOffset currentTime)
    {
        return !IsConfirmed && CreatedAt <= currentTime.AddMinutes(-10);
    }
    public void Confirm(DateTimeOffset currentTime)
    {
        if (IsExpired(currentTime))
        {
            throw new DomainException("Reservation has expired and cannot be confirmed.");
        }
        IsConfirmed = true;
    }
}