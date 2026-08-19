namespace Application.DTOs;

public record ReservationResultDto(
    Guid ReservationId, 
    int NumberOfSeats, 
    string AuditoriumName, 
    string MovieTitle);