using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Movies;

public class GetMovieByIdQueryHandler(IMovieRepository movieRepository) : IRequestHandler<GetMovieByIdQuery, Movie?>
{
    public async Task<Movie?> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        return await movieRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}