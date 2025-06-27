namespace Nerv.Repository.Tests.Repositories;

using System;
using System.Linq;
using System.Threading.Tasks;
using Nerv.Repository.Tests.TestData;
using Nerv.Repository.Tests.Fixtures;
using Xunit;

public class RepositoryTests
{
    private readonly RepositoryFixture _fixture;

    public RepositoryTests()
    {
        _fixture = RepositoryFixture.Create();
    }

    [Fact]
    public async Task AddAsync_Should_Set_CreatedAudit_Fields()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Alice" };
        await _fixture.Repository.AddAsync(user);
        await _fixture.UnitOfWork.SaveChangesAsync();

        var saved = await _fixture.Repository.FindAsync(x => x.Id == user.Id);
        var result = saved.FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal("Alice", result!.Name);
        Assert.NotEqual(default, result.CreatedOn);
        Assert.Equal(_fixture.Actor.UserId, result.CreatedBy);
    }

    [Fact]
    public async Task Update_Should_Set_UpdatedAudit_Fields()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Initial" };
        await _fixture.Repository.AddAsync(user);
        await _fixture.UnitOfWork.SaveChangesAsync();

        user.Name = "Updated";
        _fixture.Repository.Update(user);
        await _fixture.UnitOfWork.SaveChangesAsync();

        var updated = await _fixture.Repository.FindAsync(x => x.Id == user.Id);
        var result = updated.FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal("Updated", result!.Name);
        Assert.NotNull(result.UpdatedOn);
        Assert.Equal(_fixture.Actor.UserId, result.UpdatedBy);
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Correct_Page()
    {
        for (int i = 1; i <= 25; i++)
        {
            await _fixture.Repository.AddAsync(new User { Id = Guid.NewGuid(), Name = $"User{i}" });
        }
        await _fixture.UnitOfWork.SaveChangesAsync();

        var paged = await _fixture.Repository.GetPagedAsync(null, page: 2, pageSize: 10);

        Assert.Equal(10, paged.Items.Count());
        Assert.Equal(25, paged.TotalCount);
        Assert.Equal(2, paged.Page);
        Assert.Equal(10, paged.PageSize);
    }

    [Fact]
    public async Task Remove_Should_SoftDelete_Entity()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "ToDelete" };
        await _fixture.Repository.AddAsync(user);
        await _fixture.UnitOfWork.SaveChangesAsync();

        _fixture.Repository.Remove(user);
        await _fixture.UnitOfWork.SaveChangesAsync();

        var all = _fixture.Repository.Query().ToList();
        Assert.DoesNotContain(all, u => u.Id == user.Id);
    }
}
