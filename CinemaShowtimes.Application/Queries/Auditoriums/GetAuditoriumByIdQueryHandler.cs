using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Auditoriums;

public class GetAuditoriumByIdQueryHandler(IAuditoriumRepository auditoriumRepository)
    : IRequestHandler<GetAuditoriumByIdQuery, Auditorium?>
{
    public async Task<Auditorium?> Handle(GetAuditoriumByIdQuery request, CancellationToken cancellationToken)
    {
        return await auditoriumRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}