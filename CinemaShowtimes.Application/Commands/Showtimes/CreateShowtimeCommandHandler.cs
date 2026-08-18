using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Showtimes;

public class CreateShowtimeCommandHandler(
    IShowtimeRepository showtimeRepository,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateShowtimeCommand, Guid>
{
    public async Task<Guid> Handle(CreateShowtimeCommand request, CancellationToken cancellationToken)
    {
        var showtime = Showtime.Create(request.MovieId, request.AuditoriumId, request.StartTime);
        
        await showtimeRepository.AddAsync(showtime, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return showtime.Id;
    }
}