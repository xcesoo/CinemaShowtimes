using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Movies;

public class GetAllMoviesQueryHandler(IMovieRepository movieRepository)
    : IRequestHandler<GetAllMoviesQuery, IReadOnlyCollection<Movie>>
{
    public async Task<IReadOnlyCollection<Movie>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        return await movieRepository.GetAllAsync(cancellationToken); 
    }
}