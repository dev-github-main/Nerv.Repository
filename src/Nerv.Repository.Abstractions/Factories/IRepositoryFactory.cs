namespace Nerv.Repository.Abstractions.Factories;

public interface IRepositoryFactory
{
    IRepository<TEntity> GetRepository<TEntity>(string contextName) where TEntity : class;
}