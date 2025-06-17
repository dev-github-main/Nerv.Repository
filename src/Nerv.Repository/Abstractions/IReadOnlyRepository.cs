namespace Nerv.Repository.Abstractions;

using System.Linq.Expressions;

/// <summary>
/// Interface for read-only access to entities.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IReadOnlyRepository<T> where T : class
{
    /// <summary>
    /// Returns a queryable set of the specified entity.
    /// </summary>
    IQueryable<T> Query();

    /// <summary>
    /// Checks if any entity satisfies the given predicate.
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Counts the number of entities matching the given predicate.
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}