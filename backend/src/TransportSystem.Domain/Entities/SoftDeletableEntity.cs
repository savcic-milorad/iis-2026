namespace TransportSystem.Domain.Entities;

/// <summary>
/// Base entity class with soft delete support
/// Entities inheriting from this class will not be physically deleted from the database
/// </summary>
public abstract class SoftDeletableEntity : Entity
{
    /// <summary>
    /// Indicates whether the entity has been soft deleted
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Date and time when the entity was soft deleted (UTC)
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    protected SoftDeletableEntity() : base()
    {
        IsDeleted = false;
    }

    protected SoftDeletableEntity(Guid id) : base(id)
    {
        IsDeleted = false;
    }

    /// <summary>
    /// Marks the entity as deleted (soft delete)
    /// </summary>
    public void Delete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    /// <summary>
    /// Restores a soft-deleted entity
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAt = null;
        MarkAsUpdated();
    }
}
