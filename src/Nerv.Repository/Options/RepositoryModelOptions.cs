namespace Nerv.Repository.Options;

/// <summary>
/// Configuration options related to EF Core model conventions used by the repository pattern.
/// This includes options such as table name pluralization and other potential model-level behaviors.
/// </summary>
public class RepositoryModelOptions
{
    /// <summary>
    /// Enables pluralization of table names.
    /// </summary>
    public bool UsePluralization { get; set; } = true;
}
