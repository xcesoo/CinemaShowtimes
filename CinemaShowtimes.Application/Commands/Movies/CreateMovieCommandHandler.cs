using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Movies;

public class CreateMovieCommandHandler(
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateMovieCommand, Guid>
{
    public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = Movie.Create(request.Title, request.Category, request.Year);
        
        await movieRepository.AddAsync(movie, cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return movie.Id;
    }
}