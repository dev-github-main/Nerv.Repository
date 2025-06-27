namespace Nerv.Repository.Abstractions.Factories;

/// <summary>
/// Defines a factory interface responsible for providing instances of <see cref="IUnitOfWork"/>
/// based on a given context name.
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Returns an instance of <see cref="IUnitOfWork"/> associated with the specified context name.
    /// </summary>
    /// <param name="contextName">
    /// The name of the context for which a unit of work should be retrieved.
    /// This should correspond to a registered DbContext or equivalent context identifier.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IUnitOfWork"/> configured for the specified context.
    /// </returns>
    IUnitOfWork GetUnitOfWork(string contextName);
}