namespace Nerv.Repository.Options;

/// <summary>
/// Configuration options related to EF Core model conventions used by the repository pattern.
/// This includes options such as table name pluralization and other potential model-level behaviors.
/// </summary>
public class UnitOfWorkOptions
{
    /// <summary>
    /// Options related to model configuration.
    /// </summary>
    public RepositoryModelOptions ModelOptions { get; set; } = new();

    public override string ToString()
    {
        return $"PluralizeTableName: {ModelOptions.UsePluralization}";
    }
}
