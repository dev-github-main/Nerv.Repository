namespace Nerv.Repository.Tests.ServiceProvider;

using Nerv.Repository.Interfaces;
using Nerv.Repository.Options;
using Nerv.Repository.Tests.TestData;

class TestServiceProvider(TestDbContext context, UnitOfWorkOptions options) : IServiceProvider
{
    private readonly TestDbContext _context = context;
    private readonly UnitOfWorkOptions _options = options;

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IRepository<TestDbContext, User>))
            return new Repository<TestDbContext, User>(_context, _options);

        throw new InvalidOperationException($"Service not mocked: {serviceType.FullName}");
    }
}