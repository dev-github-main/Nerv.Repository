namespace Nerv.Repository.Entities;

using Nerv.Repository.Abstractions.Entities;

/// <summary>
/// Represents an abstract base class for entities with audit information.
/// </summary>
public abstract class EntityBase<TId, TUserId> : IEntity<TId>
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public TId Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public TUserId? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedOn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity.
    /// </summary>
    public TUserId? UpdatedBy { get; set; }
}
