namespace Nerv.Repository.Abstractions.Entities;

public interface IDeletableAuditable<TUserId> : IAuditable<TUserId>
{
    bool IsDeleted { get; set; }
    DateTime? DeletedOn { get; set; }
    TUserId? DeletedBy { get; set; }
}
