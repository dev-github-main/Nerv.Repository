namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions.Entities;
using System.Reflection;

/// <summary>
/// Provides extension methods for registering entities into the EF Core model builder by scanning assemblies.
/// </summary>
public static class ModelBuilderEntityRegistrar
{
    /// <summary>
    /// Scans the provided assembly and registers all non-abstract classes implementing IEntity with the model builder.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    /// <param name="assembly">The assembly to scan for entity types.</param>
    public static void RegisterEntitiesFromAssembly(this ModelBuilder modelBuilder, Assembly assembly)
    {
        var entityTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t))
            .ToList();

        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }
    }
}
