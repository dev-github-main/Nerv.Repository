namespace Nerv.Repository.Tests.Context;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Contexts;
using Nerv.Repository.Options;

public class TestDbContext(DbContextOptions options, ActorContext<Guid> actor, UnitOfWorkOptions uowOptions) : DbContextBase<Guid>(options, actor, uowOptions)
{
}
