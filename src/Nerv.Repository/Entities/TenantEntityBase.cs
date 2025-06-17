namespace Nerv.Repository.Entities;

public abstract class TenantEntityBase<TId, TUserId, TTenantId> : EntityBase<TId, TUserId>
{
    public TTenantId TenantId { get; set; } = default!;
}