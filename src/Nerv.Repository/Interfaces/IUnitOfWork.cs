namespace Nerv.Repository.Interfaces;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions;

public interface IUnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
}