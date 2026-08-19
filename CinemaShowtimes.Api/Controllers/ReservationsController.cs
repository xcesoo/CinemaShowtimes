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
    public Task<IActionResult> ReserveSeats(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id:guid}/confirm")]
    public Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}