namespace Nerv.Repository;

using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Models;

/// <summary>
/// A decorator repository that adds caching capabilities to repository queries.
/// </summary>
/// <param name="inner">The inner repository instance to decorate.</param>
/// <param name="cache">The memory cache instance used for caching query results.</param>
public class CachedRepository<T>(IRepository<T> inner, IMemoryCache cache) : IRepository<T> where T : class
{
    private readonly IRepository<T> _inner = inner;
    private readonly IMemoryCache _cache = cache;

    /// <summary>
    /// Finds entities matching the given predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The condition to filter entities.</param>
    /// <returns>A queryable of matching entities.</returns>
    public async Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _inner.FindAsync(predicate);

    /// <summary>
    /// Determines asynchronously whether any entities match the given predicate.
    /// </summary>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>True if any entity matches; otherwise, false.</returns>
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await _inner.AnyAsync(predicate);

    /// <summary>
    /// Counts asynchronously the number of entities that match the given predicate.
    /// </summary>
    /// <param name="predicate">The optional condition to filter entities.</param>
    /// <returns>The count of matching entities.</returns>
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) => await _inner.CountAsync(predicate);

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public async Task AddAsync(T entity) => await _inner.AddAsync(entity);

    /// <summary>
    /// Removes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void Remove(T entity) => _inner.Remove(entity);

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public void Update(T entity) => _inner.Update(entity);

    /// <summary>
    /// Retrieves a queryable of entities, using cached results when available.
    /// </summary>
    /// <returns>The queryable result.</returns>
    public IQueryable<T> Query()
    {
        var key = $"{typeof(T).FullName}_query";
        if (!_cache.TryGetValue(key, out IQueryable<T>? result))
        {
            result = _inner.Query();
            _cache.Set(key, result, TimeSpan.FromMinutes(5));
        }
        return result!;
    }

    /// <summary>
    /// Retrieves a paged result asynchronously for entities matching the given predicate.
    /// </summary>
    /// <param name="predicate">The optional filter condition.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>The paged result.</returns>
    public async Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>>? predicate, int page, int pageSize)
        => await _inner.GetPagedAsync(predicate, page, pageSize);
}
