namespace Nerv.Repository.Tests.Fixtures;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nerv.Repository;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Contexts;
using Nerv.Repository.Tests.Context;
using Nerv.Repository.Tests.Entities;

public class RepositoryFixture
{
    public TestDbContext Context { get; private set; }
    public ActorContext<Guid> Actor { get; private set; }
    public IRepository<User> Repository { get; private set; }
    public IUnitOfWork UnitOfWork { get; private set; }

    private RepositoryFixture()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        var actor = new ActorContext<Guid> { UserId = Guid.NewGuid() };

        Actor = actor;
        Context = new TestDbContext(options, actor);
        Context.Database.EnsureCreated();

        Repository = new Repository<User>(Context);
        UnitOfWork = new UnitOfWork(Context, new LoggerFactory().CreateLogger<UnitOfWork>());
    }

    public static RepositoryFixture Create()
    {
        return new RepositoryFixture();
    }
}
