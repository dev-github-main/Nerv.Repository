namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions.Entities;
using System.Reflection;

/// <summary>
/// Provides extension methods for the Entity Framework DbContext, including pagination and global filters.
/// </summary>
public static class DbContextExtensions
{
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
