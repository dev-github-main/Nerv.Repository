namespace Nerv.Repository.Tests.Repositories;

using System;
using Microsoft.Extensions.DependencyInjection;
using Nerv.Repository.Abstractions.Factories;
using Nerv.Repository.Contexts;
using Nerv.Repository.Extensions;
using Nerv.Repository.Interfaces;
using Nerv.Repository.Tests.TestData;
using Xunit;
using Xunit.Abstractions;

public class RepositoryServiceCollectionExtensionsTests(ITestOutputHelper output)
{
    private ITestOutputHelper _output = output;

    [Fact]
    public void AddRepositoryPattern_Should_Register_All_Dependencies()
    {
        var services = new ServiceCollection();
        var contextName = "TestContext";
        var dbContextCalled = false;

        services.AddLogging();

        services.AddRepositoryPattern<TestDbContext, Guid>(
            contextName,
            options => dbContextCalled = true,
            provider => new ActorContext<Guid> { UserId = Guid.NewGuid() },
            uowOptions => uowOptions.ModelOptions.UsePluralization = true
        );

        var provider = services.BuildServiceProvider();

        var dbContext = provider.GetService<TestDbContext>();
        var unitOfWork = provider.GetService<IUnitOfWork<TestDbContext>>();
        var actorContext = provider.GetService<ActorContext<Guid>>();
        var dataContext = provider.GetService<IDataContext<TestDbContext>>();
        var repo = unitOfWork?.Repository<User>(); // provider.GetService<IRepository<TestDbContext, User>>();

        Assert.NotNull(dbContext);
        Assert.NotNull(unitOfWork);
        Assert.NotNull(actorContext);
        Assert.NotNull(dataContext);
        Assert.NotNull(repo);
        Assert.True(dbContextCalled);

        _output.WriteLine("test completed");
    }

    [Fact]
    public void AddRepositoryFactory_Should_Register_Factories()
    {
        var services = new ServiceCollection();
        services.AddUnitOfWorkFactory();

        var provider = services.BuildServiceProvider();

        var factory = provider.GetService<IUnitOfWorkFactory>();

        Assert.NotNull(factory);

        _output.WriteLine("test completed");
    }
}
