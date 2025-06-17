using System.ComponentModel.DataAnnotations;

namespace Nerv.Repository.Entities;

public abstract class VersionedEntityBase<TId, TUserId> : EntityBase<TId, TUserId>
{
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}