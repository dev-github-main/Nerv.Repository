namespace Nerv.Repository;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Models;
using Nerv.Repository.Options;
using System.Linq.Expressions;

public class Repository<T>(IDataContext context, UnitOfWorkOptions options) : IRepository<T>, IReadOnlyRepository<T> where T : class
{
    private readonly IDataContext _context = context;
    private readonly UnitOfWorkOptions _options = options;

    public async Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await Task.FromResult(_context.Query<T>().Where(predicate));

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await _context.Query<T>().AnyAsync(predicate);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _context.Query<T>();
        return predicate is null
            ? await query.CountAsync()
            : await query.CountAsync(predicate);
    }

    public async Task AddAsync(T entity)
        => await _context.AddAsync(entity);

    public void Remove(T entity) => _context.Remove(entity);

    public void Update(T entity) => _context.Update(entity);

    public IQueryable<T> Query() => _context.Query<T>();

    public async Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>>? predicate, int page, int pageSize)
    {
        var query = _context.Query<T>();
        if (predicate != null) query = query.Where(predicate);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
