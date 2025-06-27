namespace Nerv.Repository.Interfaces;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Abstractions;

/// <summary>
/// Defines the core data access operations used by the repository and unit of work pattern.
/// This abstraction allows working with any DbContext implementation through a consistent API.
/// </summary>
public interface IDataContext<TDbContext> : IDataContext
where TDbContext : DbContext
{
}
