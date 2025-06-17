namespace Nerv.Repository.Entities;

using Nerv.Repository.Abstractions.Entities;

public abstract class DeletableEntityBase<TId, TUserId> : EntityBase<TId, TUserId>, IDeletableAuditable<TUserId>
{
    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedOn { get; set; }

    public TUserId? DeletedBy { get; set; }
}