namespace Nerv.Repository.Abstractions.Entities;

public interface IAuditable<TUserId>
{
    DateTime CreatedOn { get; set; }

    TUserId? CreatedBy { get; set; }

    DateTime? UpdatedOn { get; set; }

    TUserId? UpdatedBy { get; set; }
}