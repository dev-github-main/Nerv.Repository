namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Factories;
using Nerv.Repository.Contexts;
using Nerv.Repository.Factories;
using Nerv.Repository.Interfaces;
using Nerv.Repository.Options;

/// <summary>
/// Provides extension methods for automatic registration of repository pattern infrastructure.
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
    private static readonly Dictionary<string, Type> ContextRegistrations = [];

    /// <summary>
    /// Registers the repository pattern services including DbContext, IDataContext, IRepository, IUnitOfWork, and ActorContext.
    /// </summary>
    /// <typeparam name="TDbContext">The application's concrete DbContext, derived from DbContextBase&lt;TUserId&gt;.</typeparam>
    /// <typeparam name="TUserId">The type used for the user identifier (e.g., Guid, int).</typeparam>
    /// <param name="services">The IServiceCollection to register services with.</param>
    /// <param name="optionsAction">The action to configure the DbContext options.</param>
    /// <param name="actorFactory">Factory to resolve the current ActorContext.</param>
    /// <param name="configureOptions">Optional action to configure UnitOfWorkOptions.</param>
    /// <returns>The same IServiceCollection for chaining.</returns>
    public static IServiceCollection AddRepositoryPattern<TDbContext, TUserId>(
        this IServiceCollection services,
        string contextName,
        Action<DbContextOptionsBuilder> optionsAction,
        Func<IServiceProvider, ActorContext<TUserId>> actorFactory,
        Action<UnitOfWorkOptions>? configureOptions = null)
        where TDbContext : DbContextBase<TUserId, TDbContext>
    {
        // Register ActorContext with DI
        services.AddScoped(actorFactory);

        // Register the application's DbContext
        services.AddDbContext<TDbContext>(optionsAction);

        // Register TDbContext as IDataContext and as DbContext
        services.AddScoped<IDataContext<TDbContext>>(sp => sp.GetRequiredService<TDbContext>());
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());

        // Register UnitOfWork
        services.AddKeyedScoped<IUnitOfWork>(
            contextName,
            (sp, _) =>
               new UnitOfWork<TDbContext>(
                   sp.GetRequiredService<TDbContext>(),
                   sp.GetRequiredService<ILogger<UnitOfWork<TDbContext>>>(),
                   sp.GetRequiredService<UnitOfWorkOptions>(),
                   sp));

        services.AddScoped<IUnitOfWork<TDbContext>, UnitOfWork<TDbContext>>();

        // Register options
        var options = new UnitOfWorkOptions();
        configureOptions?.Invoke(options);
        services.AddSingleton(options);

        return services;
    }

    public static IServiceCollection AddUnitOfWorkFactory(this IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWorkFactory>(sp =>
        {
            var registry = sp.GetRequiredService<IServiceProvider>();
            return new UnitOfWorkFactory(registry);
        });

        return services;
    }

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services)
    {
        services.AddSingleton<IRepositoryFactory, RepositoryFactory>();
        return services;
    }
}
