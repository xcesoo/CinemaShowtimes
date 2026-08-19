using Application.Commands.Reservations;
using Application.Queries.Reservations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShowtimes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await mediator.Send(new GetReservationByIdQuery(id), cancellationToken);
        return reservation is not null ? Ok(reservation) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ReserveSeats(
        [FromBody] ReserveSeatsCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.ReservationId }, new { result });
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new ConfirmReservationCommand(id), cancellationToken);
        return NoContent(); 
    }
}