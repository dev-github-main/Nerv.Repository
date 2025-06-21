namespace Nerv.Repository.Contexts;

/// <summary>
/// Represents the context of an actor (user) performing operations, carrying the identifier of the current user.
/// </summary>
public sealed class ActorContext<TUserId>
{
    /// <summary>
    /// Gets or sets the identifier of the current user.
    /// </summary>
    public required TUserId UserId { get; init; }
}
