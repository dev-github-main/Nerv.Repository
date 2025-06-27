namespace Nerv.Repository.Interfaces;

using Microsoft.EntityFrameworkCore;

public interface IRepository<TDbContext, T> : Abstractions.IRepository<T>
    where TDbContext : DbContext
    where T : class
{
}