namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions.Entities;
using Nerv.Repository.Models;
using System.Reflection;

public static class DbContextExtensions
{
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

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
    }
}