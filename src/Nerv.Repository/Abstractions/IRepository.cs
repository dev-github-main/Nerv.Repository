namespace Nerv.Repository.Abstractions;

using System.Linq.Expressions;
using Nerv.Repository.Models;

/// <summary>
/// Defines generic repository operations for working with entities.
/// This interface supports basic CRUD operations and querying functionality with optional pagination.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Finds entities matching the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>An IQueryable of matching entities wrapped in a task.</returns>
    Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Determines whether any entities match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>A task representing whether any matching entities exist.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Counts the number of entities matching the optional predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The optional filter expression.</param>
    /// <returns>The total count of matching entities.</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Returns a queryable collection of all entities of the specified type.
    /// </summary>
    /// <returns>An IQueryable of the entity type.</returns>
    IQueryable<T> Query();

    /// <summary>
    /// Retrieves a paged result of entities matching the optional predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The optional filter expression.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The size of each page.</param>
    /// <returns>A task containing the paged result.</returns>
    Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>>? predicate, int page, int pageSize);
}
