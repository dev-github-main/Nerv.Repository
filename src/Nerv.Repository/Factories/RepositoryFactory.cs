namespace Nerv.Repository.Factories;

using Microsoft.Extensions.DependencyInjection;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Factories;

public class RepositoryFactory(
    IServiceProvider sp,
    IDictionary<string, Type> registrations) : IRepositoryFactory
{
    private readonly IServiceProvider _sp = sp;
    private readonly IDictionary<string, Type> _registrations = registrations;

    public IRepository<TEntity> GetRepository<TEntity>(string contextName) where TEntity : class
    {
        var dbContextType = _registrations[contextName];
        var repoType = typeof(Interfaces.IRepository<,>).MakeGenericType(dbContextType, typeof(TEntity));
        return (IRepository<TEntity>)_sp.GetRequiredService(repoType);
    }
}