namespace LunchNear.Infrastructure.Persistence.Interceptors;

using LunchNear.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// Stamps CreatedAt on every IAuditableEntity from a single TimeProvider, replacing the previous
/// inconsistency where one service used DateTime.Now and another used DateTime.UtcNow.
/// </summary>
public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;

    public AuditableEntitySaveChangesInterceptor(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = _timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
        }
    }
}
