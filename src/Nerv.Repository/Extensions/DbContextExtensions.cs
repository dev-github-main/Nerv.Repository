namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions.Entities;
using Nerv.Repository.Models;
using System.Reflection;

/// <summary>
/// Provides extension methods for the Entity Framework DbContext, including pagination and global filters.
/// </summary>
public static class DbContextExtensions
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

    /// <summary>
    /// Applies global filters to all entities implementing IDeletableAuditable, enabling soft-delete behavior.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void ApplyGlobalFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var interfaces = clrType.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IDeletableAuditable<>))
                {
                    var method = typeof(DbContextExtensions)
                        .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.Static | BindingFlags.NonPublic)!
                        .MakeGenericMethod(clrType);

                    method.Invoke(null, new object[] { modelBuilder });
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Configures the query filter for soft-deleted entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="modelBuilder">The model builder.</param>
    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
    }
}
