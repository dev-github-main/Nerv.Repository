using System.ComponentModel.DataAnnotations;

namespace Nerv.Repository.Entities;

/// <summary>
/// Represents an abstract base class for entities that include optimistic concurrency control via a row version.
/// </summary>
public abstract class VersionedEntityBase<TId, TUserId> : EntityBase<TId, TUserId>
{
    /// <summary>
    /// Gets or sets the row version used for optimistic concurrency control.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
