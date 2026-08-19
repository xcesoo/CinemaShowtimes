using Application.Commands.Movies;
using Application.Queries.Movies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShowtimes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var movies = await mediator.Send(new GetAllMoviesQuery(), cancellationToken);
        return Ok(movies);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var movie = await mediator.Send(new GetMovieByIdQuery(id), cancellationToken);
        return movie is not null ? Ok(movie) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMovieCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}