namespace Nerv.Repository.Contexts;

public sealed class ActorContext<TUserId>
{
    public required TUserId UserId { get; init; }
}
