namespace MyWarehouse.Application.Common.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
        : base("Entity was not found.")
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public EntityNotFoundException(string entityName, object entityId)
        : base($"Entity '{entityName}' with ID '{entityId}' was not found.")
    {
    }
}
