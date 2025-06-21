namespace Nerv.Repository.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Contexts;
using Nerv.Repository.Options;

/// <summary>
/// Provides extension methods for automatic registration of repository pattern infrastructure.
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
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
        Action<DbContextOptionsBuilder> optionsAction,
        Func<IServiceProvider, ActorContext<TUserId>> actorFactory,
        Action<UnitOfWorkOptions>? configureOptions = null)
        where TDbContext : DbContextBase<TUserId>
    {
        // Register ActorContext with DI
        services.AddScoped(actorFactory);

        // Register the application's DbContext
        services.AddDbContext<TDbContext>(optionsAction);

        // Register DbContext as IDataContext
        services.AddScoped<IDataContext>(sp => sp.GetRequiredService<TDbContext>());

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register generic repository types
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));

        // Register options
        var options = new UnitOfWorkOptions();
        configureOptions?.Invoke(options);
        services.AddSingleton(options);

        return services;
    }
}
