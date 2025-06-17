namespace Nerv.Repository;

using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Models;

public class CachedRepository<T> : IRepository<T> where T : class
{
    private readonly IRepository<T> _inner;
    private readonly IMemoryCache _cache;

    public CachedRepository(IRepository<T> inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _inner.FindAsync(predicate);
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await _inner.AnyAsync(predicate);
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) => await _inner.CountAsync(predicate);
    public async Task AddAsync(T entity) => await _inner.AddAsync(entity);
    public void Remove(T entity) => _inner.Remove(entity);
    public void Update(T entity) => _inner.Update(entity);
    public IQueryable<T> Query()
    {
        var key = $"{typeof(T).FullName}_query";
        if (!_cache.TryGetValue(key, out IQueryable<T> result))
        {
            result = _inner.Query();
            _cache.Set(key, result, TimeSpan.FromMinutes(5));
        }
        return result;
    }

    public async Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>>? predicate, int page, int pageSize)
        => await _inner.GetPagedAsync(predicate, page, pageSize);
}