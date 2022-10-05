namespace MyWarehouse.Domain.Common;

public interface IAudited
{
    string CreatedBy { get; }

    DateTime CreatedAt { get; }

    string? LastModifiedBy { get; }

    DateTime? LastModifiedAt { get; }
}
