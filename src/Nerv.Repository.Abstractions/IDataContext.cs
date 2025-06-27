namespace Nerv.Repository.Abstractions;

/// <summary>
/// Defines the core data access operations used by the repository and unit of work pattern.
/// This abstraction allows working with any DbContext implementation through a consistent API.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a queryable collection of entities for the specified type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>An IQueryable of the specified entity type.</returns>
    IQueryable<T> Query<T>() where T : class;

    /// <summary>
    /// Adds a new entity instance to the context asynchronously.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Marks an existing entity as modified.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity to update.</param>
    void Update<T>(T entity) where T : class;

    /// <summary>
    /// Marks an entity for deletion.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity to remove.</param>
    void Remove<T>(T entity) where T : class;

    /// <summary>
    /// Applies any pending database migrations for this context.
    /// Intended to be called during application startup or deployment.
    /// </summary>
    void ApplyDatabaseMigrations();
}
