namespace Nerv.Repository;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions;
using System.Linq.Expressions;

/// <summary>
/// Read-only implementation of a generic repository.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public class ReadOnlyRepository<T>(IDataContext context) : IReadOnlyRepository<T> where T : class
{
    private readonly IDataContext _context = context;

    public IQueryable<T> Query() => _context.Query<T>();

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await _context.Query<T>().AnyAsync(predicate);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _context.Query<T>();
        return predicate is null
            ? await query.CountAsync()
            : await query.CountAsync(predicate);
    }
}
