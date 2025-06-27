namespace Nerv.Repository.Abstractions;

/// <summary>
/// Defines the Unit of Work abstraction, encapsulating transactional operations and repository access.
/// This interface coordinates the work of multiple repositories under a single transaction boundary.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Provides a repository instance for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The repository for the specified entity type.</returns>
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Saves all changes made across the tracked repositories within this unit of work asynchronously.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync();
}
