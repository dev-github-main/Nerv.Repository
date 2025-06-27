namespace Nerv.Repository.Tests.Fixtures;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nerv.Repository;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Contexts;
using Nerv.Repository.Tests.TestData;
using Nerv.Repository.Tests.Helpers;
using Nerv.Repository.Tests.ServiceProvider;

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

        var uowOptions = TestUnitOfWorkOptionsFactory.Create();

        Actor = actor;
        Context = new TestDbContext(options, actor, uowOptions);
        Context.Database.EnsureCreated();

        Repository = new Repository<TestDbContext, User>(Context, uowOptions);
        UnitOfWork = new UnitOfWork<TestDbContext>(
            Context,
            new LoggerFactory().CreateLogger<UnitOfWork<TestDbContext>>(),
            uowOptions,
            new TestServiceProvider(Context, uowOptions));
    }

    public static RepositoryFixture Create()
    {
        return new RepositoryFixture();
    }
}
