namespace Nerv.Repository.Tests.TestData;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Contexts;
using Nerv.Repository.Options;

public class TestDbContext(
    DbContextOptions options,
    ActorContext<Guid> actor,
    UnitOfWorkOptions uowOptions) : DbContextBase<Guid, TestDbContext>(options, actor, uowOptions)
{
}
