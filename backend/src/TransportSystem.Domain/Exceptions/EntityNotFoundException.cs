namespace TransportSystem.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found in the system
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public Guid EntityId { get; }

    public EntityNotFoundException(string entityName, Guid id)
        : base($"{entityName} with ID '{id}' was not found")
    {
        EntityName = entityName;
        EntityId = id;
    }

    public EntityNotFoundException(string entityName, Guid id, Exception innerException)
        : base($"{entityName} with ID '{id}' was not found", innerException)
    {
        EntityName = entityName;
        EntityId = id;
    }
}
