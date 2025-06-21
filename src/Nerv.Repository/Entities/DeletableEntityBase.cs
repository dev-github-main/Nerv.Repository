namespace Nerv.Repository.Entities;

using Nerv.Repository.Abstractions.Entities;

/// <summary>
/// Represents an abstract base class for entities that support soft deletion and audit tracking.
/// </summary>
public abstract class DeletableEntityBase<TId, TUserId> : EntityBase<TId, TUserId>, IDeletableAuditable<TUserId>
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity has been soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the date and time when the entity was deleted.
    /// </summary>
    public DateTime? DeletedOn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    public TUserId? DeletedBy { get; set; }
}
