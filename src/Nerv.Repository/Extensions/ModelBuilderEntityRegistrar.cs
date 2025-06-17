namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions.Entities;
using System.Reflection;

public static class ModelBuilderEntityRegistrar
{
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
