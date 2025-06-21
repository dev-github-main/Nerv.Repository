namespace Nerv.Repository.Entities;

/// <summary>
/// Represents an abstract base class for entities associated with a tenant, inheriting audit information.
/// </summary>
public abstract class TenantEntityBase<TId, TUserId, TTenantId> : EntityBase<TId, TUserId>
{
    /// <summary>
    /// Gets or sets the identifier of the tenant to which the entity belongs.
    /// </summary>
    public TTenantId TenantId { get; set; } = default!;
}
