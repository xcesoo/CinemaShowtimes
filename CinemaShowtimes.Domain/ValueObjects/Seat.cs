namespace Domain.ValueObjects;

public record Seat
{
    public short Row { get; private set; }
    public short Number { get; private set; }
    
    private Seat(){} //for ef core

    public static Seat Create(short row, short number)
    {
        if (row < 1 || number < 1) throw new ArgumentOutOfRangeException();
        
        return new Seat()
        {
            Row = row,
            Number = number
        };
    }
}