namespace Nerv.Repository.Tests.UnitOfWork;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nerv.Repository.Abstractions;
using Nerv.Repository.Abstractions.Factories;
using Nerv.Repository.Contexts;
using Nerv.Repository.Extensions;
using Nerv.Repository.Tests.TestData;
using Xunit;

public class UnitOfWorkFactoryTests
{
    private readonly IServiceProvider _provider;

    public UnitOfWorkFactoryTests()
    {
        var services = new ServiceCollection();

        services.AddLogging();
                
        services.AddRepositoryPattern<TestDbContext, Guid>(
            contextName: "Test",
            optionsAction: opt => opt.UseInMemoryDatabase("TUW_" + Guid.NewGuid()),
            actorFactory: _ => new ActorContext<Guid> { UserId = Guid.Empty },
            configureOptions: _ => { }
        );
        
        services.AddUnitOfWorkFactory();

        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public void Factory_Returns_Correct_UnitOfWork_Instance()
    {
        var factory = _provider.GetRequiredService<IUnitOfWorkFactory>();
        var uow = factory.GetUnitOfWork("Test");

        Assert.NotNull(uow);
        Assert.IsAssignableFrom<IUnitOfWork>(uow);
    }

    [Fact]
    public async Task Factory_Throws_On_Invalid_ContextName()
    {
        var factory = _provider.GetRequiredService<IUnitOfWorkFactory>();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await Task.Run(() => factory.GetUnitOfWork("Unknown")));

        Assert.Contains("No UnitOfWork registered for context", ex.Message);
    }
}
