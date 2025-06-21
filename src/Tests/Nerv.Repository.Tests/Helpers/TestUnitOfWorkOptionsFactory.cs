namespace Nerv.Repository.Tests.Helpers;

using Nerv.Repository.Options;

public static class TestUnitOfWorkOptionsFactory
{
    public static UnitOfWorkOptions Create()
    {
        return new UnitOfWorkOptions
        {
            ModelOptions = new RepositoryModelOptions
            {
                UsePluralization = false // Pluralization disabled for tests
            }
        };
    }
}
