namespace Nerv.Repository.Tests.Context;

using Microsoft.EntityFrameworkCore;
using Nerv.Repository.Contexts;

public class TestDbContext(DbContextOptions options, ActorContext<Guid> actor) : DbContextBase<Guid>(options, actor)
{
}