using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Showtimes;

public class GetAllShowtimesQueryHandler(IShowtimeRepository showtimeRepository)
    : IRequestHandler<GetAllShowtimesQuery, IReadOnlyCollection<Showtime>>
{
    public async Task<IReadOnlyCollection<Showtime>> Handle(GetAllShowtimesQuery request, CancellationToken cancellationToken)
    {
        return await showtimeRepository.GetAllAsync(cancellationToken);
    }
}