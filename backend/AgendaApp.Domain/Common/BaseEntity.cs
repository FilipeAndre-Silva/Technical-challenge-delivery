namespace AgendaApp.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    public Guid CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }

    protected void SetCreatedBy(Guid createdBy)
    {
        CreatedBy = createdBy;
    }

    protected void SetCreatedAt()
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected BaseEntity(Guid createdBy)
    {
        Id = Guid.NewGuid();
        SetCreatedBy(createdBy);
        SetCreatedAt();
    }

    protected BaseEntity()
    {
    }
}