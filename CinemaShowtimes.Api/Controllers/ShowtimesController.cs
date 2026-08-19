using Application.Commands.Showtimes;
using Application.Queries.Showtimes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShowtimes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var showtimes = await mediator.Send(new GetAllShowtimesQuery(), cancellationToken);
        return Ok(showtimes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var showtime = await mediator.Send(new GetShowtimeByIdQuery(id), cancellationToken);
        return showtime is not null ? Ok(showtime) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShowtimeCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}/seats")]
    public Task<IActionResult> GetSeatMap(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}