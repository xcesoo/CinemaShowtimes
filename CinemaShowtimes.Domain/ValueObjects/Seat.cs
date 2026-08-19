using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Seat
{
    public short Row { get; private set; }
    public short Number { get; private set; }
    
    private Seat(){} //for ef core

    public static Seat Create(short row, short number)
    {
        if (row < 1 || number < 1) throw new DomainException($"Invalid seat R{row} N{number}");
        
        return new Seat()
        {
            Row = row,
            Number = number
        };
    }
}