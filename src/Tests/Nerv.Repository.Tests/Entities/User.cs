namespace Nerv.Repository.Tests.Entities;

using Nerv.Repository.Entities;

public class User : DeletableEntityBase<Guid, Guid>
{
    public string Name { get; set; } = default!;
}
