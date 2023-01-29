namespace Domain.Contracts;

public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
{
    // TODO: Fully implement auditable entity to ensure full auditing trail on any inheriting entity
}

public interface IAuditableEntity : IEntity
{
    Guid CreatedBy { get; set; }

    DateTime CreatedOn { get; set; }

    Guid? LastModifiedBy { get; set; }

    DateTime? LastModifiedOn { get; set; }
}
