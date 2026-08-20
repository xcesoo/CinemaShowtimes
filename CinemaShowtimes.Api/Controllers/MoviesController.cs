using Application.Commands.Movies;
using Application.Queries.Movies;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShowtimes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Movie>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var movies = await mediator.Send(new GetAllMoviesQuery(), cancellationToken);
        return Ok(movies);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var movie = await mediator.Send(new GetMovieByIdQuery(id), cancellationToken);
        return movie is not null ? Ok(movie) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}