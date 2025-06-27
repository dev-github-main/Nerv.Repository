namespace Nerv.Repository.Tests.Fixtures;

using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nerv.Repository;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Contexts;
using Nerv.Repository.Tests.TestData;
using Nerv.Repository.Tests.Helpers;
using Nerv.Repository.Tests.ServiceProvider;

public class UnitOfWorkFixture : IDisposable
{
    public SqliteConnection Connection { get; private set; }
    public TestDbContext Context { get; private set; }
    public ActorContext<Guid> Actor { get; private set; }
    public IRepository<User> Repository { get; private set; }
    public IUnitOfWork UnitOfWork { get; private set; }

    private UnitOfWorkFixture()
    {
        Connection = new SqliteConnection("Filename=:memory:");
        Connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(Connection)
            .EnableSensitiveDataLogging()
            .Options;

        var actor = new ActorContext<Guid> { UserId = Guid.NewGuid() };
        Actor = actor;

        var uowOptions = TestUnitOfWorkOptionsFactory.Create();

        Context = new TestDbContext(options, actor, uowOptions);
        Context.Database.EnsureCreated();

        Repository = new Repository<TestDbContext, User>(Context, uowOptions);
        UnitOfWork = new UnitOfWork<TestDbContext>(
            Context,
            new LoggerFactory().CreateLogger<UnitOfWork<TestDbContext>>(),
            uowOptions,
            new TestServiceProvider(Context, uowOptions));
    }

    public static UnitOfWorkFixture Create()
    {
        return new UnitOfWorkFixture();
    }

    public void Dispose()
    {
        Context.Dispose();
        Connection.Dispose();
    }
}
