namespace Nerv.Repository.Abstractions.Diagnostics;

/// <summary>
/// Provides diagnostic information for UnitOfWork instances.
/// </summary>
public interface IUnitOfWorkDiagnostics
{
    /// <summary>
    /// Gets the collection of active repositories currently instantiated by the UnitOfWork.
    /// </summary>
    IReadOnlyDictionary<Type, object> ActiveRepositories { get; }

    /// <summary>
    /// Indicates whether a transaction is currently active.
    /// </summary>
    bool IsTransactionActive { get; }
}
