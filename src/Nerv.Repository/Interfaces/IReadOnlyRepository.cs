namespace Nerv.Repository.Interfaces;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions;

public interface IReadOnlyRepository<TDbContext, T> : IReadOnlyRepository<T>, IRepository<TDbContext, T>
    where TDbContext : DbContext
    where T : class
{
}