using Domain.Entities;
using MediatR;

namespace Application.Commands.Movies;

public readonly record struct CreateMovieCommand(string Title, string Category, int Year) : IRequest<Guid>;