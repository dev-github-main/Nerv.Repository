namespace Nerv.Repository.Abstractions;

public interface IDataContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    IQueryable<T> Query<T>() where T : class;

    Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

    void Update<T>(T entity) where T : class;

    void Remove<T>(T entity) where T : class;

    void ApplyDatabaseMigrations();
}