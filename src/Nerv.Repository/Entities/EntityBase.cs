namespace Nerv.Repository.Entities;

using Nerv.Repository.Abstractions.Entities;

public abstract class EntityBase<TId, TUserId> : IEntity<TId>
{
    public TId Id { get; set; } = default!;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public TUserId? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public TUserId? UpdatedBy { get; set; }
}