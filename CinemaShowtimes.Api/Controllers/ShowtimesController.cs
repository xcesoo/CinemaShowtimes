using Application.Commands.Showtimes;
using Application.DTOs;
using Application.Queries.Showtimes;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShowtimes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Showtime>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var showtimes = await mediator.Send(new GetAllShowtimesQuery(), cancellationToken);
        return Ok(showtimes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Showtime), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var showtime = await mediator.Send(new GetShowtimeByIdQuery(id), cancellationToken);
        return showtime is not null ? Ok(showtime) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateShowtimeCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}/seats")]
    [ProducesResponseType(typeof(ShowtimeSeatMapDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeatMap(Guid id, CancellationToken cancellationToken)
    {
        var seatMap = await mediator.Send(new GetShowtimeSeatMapQuery(id), cancellationToken);
        return seatMap is not null ? Ok(seatMap) : NotFound();
    }
}