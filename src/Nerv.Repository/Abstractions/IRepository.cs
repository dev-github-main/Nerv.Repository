namespace Nerv.Repository.Abstractions;

using System.Linq.Expressions;
using Nerv.Repository.Models;

public interface IRepository<T> where T : class
{
    Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    Task AddAsync(T entity);

    void Remove(T entity);

    void Update(T entity);

    IQueryable<T> Query();

    Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>>? predicate, int page, int pageSize);
}