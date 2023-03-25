namespace MyWarehouse.Domain.Common;

public interface ISoftDeletable
{
    public string? DeletedBy { get; }

    public DateTime? DeletedAt { get; }
}
