using Domain.Entities;
using MediatR;

namespace Application.Queries.Auditoriums;

public readonly record struct GetAuditoriumByIdQuery(Guid Id) :IRequest<Auditorium?>;