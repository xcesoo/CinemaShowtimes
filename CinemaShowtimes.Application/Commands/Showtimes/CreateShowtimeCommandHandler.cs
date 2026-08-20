using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Showtimes;

public class CreateShowtimeCommandHandler(
    IShowtimeRepository showtimeRepository,
    IMovieRepository movieRepository,
    IAuditoriumRepository auditoriumRepository,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateShowtimeCommand, Guid>
{
    public async Task<Guid> Handle(CreateShowtimeCommand request, CancellationToken cancellationToken)
    {
        _ = await movieRepository.GetByIdAsync(
                request.MovieId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Movie with ID {request.MovieId} not found.");
        
        _ = await auditoriumRepository.GetByIdAsync(
                request.AuditoriumId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Auditorium with ID {request.AuditoriumId} not found.");
        
        var showtime = Showtime.Create(request.MovieId, request.AuditoriumId, request.StartTime);
        
        await showtimeRepository.AddAsync(showtime, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return showtime.Id;
    }
}