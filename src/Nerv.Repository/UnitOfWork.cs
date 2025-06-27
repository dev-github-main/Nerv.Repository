namespace Nerv.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Diagnostics;
using Nerv.Repository.Interfaces;
using Nerv.Repository.Options;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides an implementation of the Unit of Work pattern for managing repository instances and transactions.
/// </summary>
/// <param name="dbContext">The database context instance.</param>
/// <param name="logger">The logger instance for UnitOfWork.</param>
/// <param name="options">The configuration options for UnitOfWork and repository behavior.</param>
/// <param name="serviceProvider">The service provider for resolving repository instances.</param>
public class UnitOfWork<TDbContext>(
    TDbContext dbContext,
    ILogger<UnitOfWork<TDbContext>> logger,
    UnitOfWorkOptions options,
    IServiceProvider serviceProvider) : IUnitOfWork<TDbContext>, IUnitOfWorkDiagnostics
where TDbContext : DbContext, IDataContext<TDbContext>
{
    private readonly TDbContext _dbContext = dbContext;
    private readonly ILogger<UnitOfWork<TDbContext>> _logger = logger;
    private readonly UnitOfWorkOptions _options = options;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
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
        var key = typeof(T);

        if (!_repositories.TryGetValue(key, out object? value))
        {
            var repo = new Repository<TDbContext, T>(
                _dbContext,
                _options);

            value = repo;
            _repositories[key] = value;
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
