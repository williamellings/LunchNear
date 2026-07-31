namespace LunchNear.Domain.Common;

/// <summary>
/// Marks an entity whose creation timestamp is managed by infrastructure (a SaveChanges interceptor)
/// rather than by domain logic, so every entity gets its CreatedAt stamped consistently in one place.
/// </summary>
public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
}
