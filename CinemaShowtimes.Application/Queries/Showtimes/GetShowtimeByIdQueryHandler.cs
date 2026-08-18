using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Showtimes;

public class GetShowtimeByIdQueryHandler(IShowtimeRepository showtimeRepository)
    : IRequestHandler<GetShowtimeByIdQuery, Showtime?>
{
    public async Task<Showtime?> Handle(GetShowtimeByIdQuery request, CancellationToken cancellationToken)
    {
        return await showtimeRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}