namespace Nerv.Repository;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Entities;
using Nerv.Repository.Contexts;
using Nerv.Repository.Extensions;

public abstract class DbContextBase<TUserId>(DbContextOptions options, ActorContext<TUserId> actor) : DbContext(options), IDataContext
{
    private readonly ActorContext<TUserId> _actor = actor;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RegisterEntitiesFromAssembly(GetType().Assembly);
        modelBuilder.ApplyGlobalFilters();
        base.OnModelCreating(modelBuilder);
    }

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

    public IQueryable<T> Query<T>() where T : class => Set<T>();

    async Task IDataContext.AddAsync<T>(T entity, CancellationToken cancellationToken) => await Set<T>().AddAsync(entity, cancellationToken);

    void IDataContext.Update<T>(T entity) => Set<T>().Update(entity);

    void IDataContext.Remove<T>(T entity) => Set<T>().Remove(entity);
}