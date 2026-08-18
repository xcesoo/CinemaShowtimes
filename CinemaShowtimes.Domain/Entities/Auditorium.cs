using Domain.ValueObjects;

namespace Domain.Entities;

public class Auditorium
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    
    private readonly List<Seat> _seats = new();
    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();
    
    private Auditorium(){} //for ef core

    public static Auditorium Create (string name, IEnumerable<Seat> seats)
    {
        ArgumentNullException.ThrowIfNull(seats);
        
        var auditorium = new Auditorium()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
        };
        auditorium._seats.AddRange(seats);
        return auditorium;
    }
}