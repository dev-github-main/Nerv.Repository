namespace Nerv.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Diagnostics;
using Nerv.Repository.Options;

/// <summary>
/// Provides an implementation of the Unit of Work pattern for managing repository instances and transactions.
/// </summary>
/// <param name="dbContext">The database context instance.</param>
/// <param name="logger">The logger instance for UnitOfWork.</param>
/// <param name="options">The configuration options for UnitOfWork and repository behavior.</param>
public class UnitOfWork(DbContext dbContext, ILogger<UnitOfWork> logger, UnitOfWorkOptions options) : IUnitOfWork, IUnitOfWorkDiagnostics
{
    private readonly DbContext _dbContext = dbContext;
    private readonly ILogger<UnitOfWork> _logger = logger;
    private readonly UnitOfWorkOptions _options = options;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories = [];

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, object> ActiveRepositories => _repositories;

    /// <inheritdoc />
    public bool IsTransactionActive => _transaction != null;

    /// <summary>
    /// Retrieves a repository instance for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The repository instance for the entity type.</returns>
    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out object? value))
        {
            var repoType = typeof(Repository<>).MakeGenericType(type);
            var instance = Activator.CreateInstance(repoType, _dbContext, _options)!;
            value = instance;
            _repositories[type] = value;
        }

        return (IRepository<T>)value;
    }

    /// <summary>
    /// Saves all pending changes in the database context.
    /// </summary>
    /// <returns>The number of affected rows.</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction ??= await _dbContext.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the database context and any active transaction.
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext.Dispose();
    }
}
