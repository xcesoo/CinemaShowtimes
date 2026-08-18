namespace Domain.Entities;

public class Showtime
{
    public Guid Id { get; init; }
    public Guid MovieId { get; private set; }
    public Guid AuditoriumId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    
    private Showtime (){} //for ef core

    public static Showtime Create(Guid movieId, Guid auditoriumId, DateTimeOffset startTime)
    {
        return new Showtime()
        {
            Id = Guid.CreateVersion7(),
            MovieId = movieId,
            AuditoriumId = auditoriumId,
            StartTime = startTime,
        };
    }
}