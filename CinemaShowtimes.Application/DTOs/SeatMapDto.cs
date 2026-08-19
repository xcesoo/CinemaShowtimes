namespace Application.DTOs;

public enum SeatStatus
{
    Available = 0,
    Reserved = 1,
    Sold = 2
}

public record SeatDto(short Row, short Number, SeatStatus Status);

public record ShowtimeSeatMapDto(
    Guid ShowtimeId,
    string AuditoriumName,
    IEnumerable<SeatDto> Seats);