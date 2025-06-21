namespace Nerv.Repository;

using Microsoft.EntityFrameworkCore;
using Humanizer;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Entities;
using Nerv.Repository.Contexts;
using Nerv.Repository.Extensions;
using Nerv.Repository.Options;

/// <summary>
/// Abstract base class for Entity Framework Core DbContext implementations using the repository and unit of work pattern.
/// This base class integrates auditing, soft-delete handling, global filters, automatic pluralization of table names,
/// and standardized entity registration from assembly scanning. It also injects the current ActorContext and allows
/// customization of model configuration through UnitOfWorkOptions.
/// </summary>
/// <typeparam name="TUserId">The type used for representing the user identifier in audit fields.</typeparam>
public abstract class DbContextBase<TUserId>(DbContextOptions options, ActorContext<TUserId> actor, UnitOfWorkOptions uowOptions)
    : DbContext(options), IDataContext
{
    private readonly ActorContext<TUserId> _actor = actor;
    private readonly UnitOfWorkOptions _uowOptions = uowOptions;

    /// <summary>
    /// Configures the entity model for EF Core, including registering entities, applying global filters,
    /// and optionally applying table name pluralization based on the provided options.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RegisterEntitiesFromAssembly(GetType().Assembly);
        modelBuilder.ApplyGlobalFilters();

        if (_uowOptions.ModelOptions.UsePluralization)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var currentTableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(currentTableName))
                {
                    entity.SetTableName(Pluralize(currentTableName));
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// Applies auditing fields for entities implementing IAuditable and IDeletableAuditable interfaces.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var userId = _actor.UserId;

        foreach (var entry in ChangeTracker.Entries())
        {
            var entityType = entry.Entity.GetType();
            var interfaces = entityType.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.IsGenericType)
                {
                    var def = iface.GetGenericTypeDefinition();

                    if (def == typeof(IAuditable<>))
                    {
                        entry.CurrentValues[nameof(IAuditable<TUserId>.UpdatedOn)] = now;
                        entry.CurrentValues[nameof(IAuditable<TUserId>.UpdatedBy)] = userId;

                        if (entry.State == EntityState.Added)
                        {
                            entry.CurrentValues[nameof(IAuditable<TUserId>.CreatedOn)] = now;
                            entry.CurrentValues[nameof(IAuditable<TUserId>.CreatedBy)] = userId;
                        }
                    }
                    else if (def == typeof(IDeletableAuditable<>))
                    {
                        if (entry.State == EntityState.Deleted)
                        {
                            entry.State = EntityState.Modified;
                            entry.CurrentValues[nameof(IDeletableAuditable<TUserId>.IsDeleted)] = true;
                            entry.CurrentValues[nameof(IDeletableAuditable<TUserId>.DeletedOn)] = now;
                            entry.CurrentValues[nameof(IDeletableAuditable<TUserId>.DeletedBy)] = userId;
                        }
                    }
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Returns a queryable set for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A queryable collection of entities.</returns>
    public IQueryable<T> Query<T>() where T : class => Set<T>();

    /// <summary>
    /// Adds a new entity to the context asynchronously.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    async Task IDataContext.AddAsync<T>(T entity, CancellationToken cancellationToken) => await Set<T>().AddAsync(entity, cancellationToken);

    /// <summary>
    /// Updates an existing entity in the context.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity to update.</param>
    void IDataContext.Update<T>(T entity) => Set<T>().Update(entity);

    /// <summary>
    /// Removes an entity from the context.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity to remove.</param>
    void IDataContext.Remove<T>(T entity) => Set<T>().Remove(entity);

    /// <summary>
    /// Applies any pending migrations for the context to the database.
    /// </summary>
    void IDataContext.ApplyDatabaseMigrations()
    {
        Database.Migrate();
    }

    /// <summary>
    /// Pluralizes the table name using Humanizer library.
    /// </summary>
    /// <param name="name">The singular name.</param>
    /// <returns>The pluralized name.</returns>
    private static string Pluralize(string name)
    {
        return name.Pluralize();
    }
}
