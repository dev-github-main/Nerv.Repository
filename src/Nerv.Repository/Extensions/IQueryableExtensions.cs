namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions.Models;

/// <summary>
/// Provides extension methods for IQueryable.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Asynchronously returns a paged result from the given query.
    /// </summary>
    /// <typeparam name="T">The type of the query element.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A paged result containing the requested items and pagination metadata.</returns>
    public static async Task<PagedResult<T>> ToPagedAsync<T>(this IQueryable<T> query, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = count,
            Page = page,
            PageSize = pageSize
        };
    }
}