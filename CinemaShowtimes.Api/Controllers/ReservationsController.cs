using Application.Commands.Reservations;
using Application.DTOs;
using Application.Queries.Reservations;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaShowtimes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await mediator.Send(new GetReservationByIdQuery(id), cancellationToken);
        return reservation is not null ? Ok(reservation) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReservationResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReserveSeats(
        [FromBody] ReserveSeatsCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.ReservationId }, result );
    }

    [HttpPatch("{id:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new ConfirmReservationCommand(id), cancellationToken);
        return NoContent(); 
    }
    
    [HttpPost("contiguous")]
    [ProducesResponseType(typeof(ReservationResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReserveContiguousSeats(
        [FromBody] ReserveContiguousSeatsCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.ReservationId }, result);
    }
}