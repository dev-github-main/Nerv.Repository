namespace Nerv.Repository.Factories;

using Microsoft.Extensions.DependencyInjection;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Factories;

public class UnitOfWorkFactory(IServiceProvider provider) : IUnitOfWorkFactory
{
    private readonly IServiceProvider _provider = provider;

public IUnitOfWork GetUnitOfWork(string contextName)
{
    var service = _provider.GetKeyedService<IUnitOfWork>(contextName);
    
    if (service is not IUnitOfWork uow)
            throw new InvalidOperationException($"No UnitOfWork registered for context '{contextName}'");

    return uow;
}
}
