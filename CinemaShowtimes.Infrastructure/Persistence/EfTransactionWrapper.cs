using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CinemaShowtimes.Infrastructure.Persistence;

internal class EfTransactionWrapper(IDbContextTransaction transaction) : IDomainTransaction
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await transaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await transaction.DisposeAsync();
    }
}