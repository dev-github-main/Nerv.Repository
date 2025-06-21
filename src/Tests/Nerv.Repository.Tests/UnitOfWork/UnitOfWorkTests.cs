namespace Nerv.Repository.Tests.UnitOfWork;

using System;
using System.Linq;
using System.Threading.Tasks;

using Nerv.Repository.Tests.Entities;
using Nerv.Repository.Tests.Fixtures;

using Xunit;

public class UnitOfWorkTests
{
    [Fact]
    public async Task UnitOfWork_Should_Rollback_Transaction()
    {
        using var fixture = UnitOfWorkFixture.Create();

        var user = new User { Id = Guid.NewGuid(), Name = "BeforeRollback" };

        await fixture.UnitOfWork.BeginTransactionAsync();
        await fixture.Repository.AddAsync(user);
        await fixture.UnitOfWork.RollbackTransactionAsync();

        var result = await fixture.Repository.FindAsync(x => x.Id == user.Id);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UnitOfWork_Should_Commit_Transaction()
    {
        using var fixture = UnitOfWorkFixture.Create();

        var user = new User { Id = Guid.NewGuid(), Name = "CommitTest" };

        await fixture.UnitOfWork.BeginTransactionAsync();
        await fixture.Repository.AddAsync(user);
        await fixture.UnitOfWork.SaveChangesAsync();
        await fixture.UnitOfWork.CommitTransactionAsync();

        var result = await fixture.Repository.FindAsync(x => x.Id == user.Id);
        Assert.Single(result);
        Assert.Equal("CommitTest", result.First().Name);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Save_Without_Transaction()
    {
        using var fixture = UnitOfWorkFixture.Create();

        var user = new User { Id = Guid.NewGuid(), Name = "NoTransaction" };
        await fixture.Repository.AddAsync(user);
        await fixture.UnitOfWork.SaveChangesAsync();

        var result = await fixture.Repository.FindAsync(x => x.Id == user.Id);
        Assert.Single(result);
    }

    [Fact]
    public async Task Multiple_UnitOfWorks_Should_Not_Conflict()
    {
        using var fixture1 = UnitOfWorkFixture.Create();
        using var fixture2 = UnitOfWorkFixture.Create();

        await fixture1.Repository.AddAsync(new User { Id = Guid.NewGuid(), Name = "User1" });
        await fixture1.UnitOfWork.SaveChangesAsync();

        await fixture2.Repository.AddAsync(new User { Id = Guid.NewGuid(), Name = "User2" });
        await fixture2.UnitOfWork.SaveChangesAsync();

        var count1 = await fixture1.Repository.CountAsync();
        var count2 = await fixture2.Repository.CountAsync();

        Assert.True(count1 >= 1);
        Assert.True(count2 >= 1);
    }
}
