using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Domain.Common;

public abstract class BaseEntity :IEntity
{
    public int Id { get; set; }
    public BaseEntity()
    {
        
    }
    public BaseEntity(int id) : this()
    {
        Id = id;
    }
    private readonly List<BaseIntegrationEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseIntegrationEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseIntegrationEvent domainIntegrationEvent)
    {
        _domainEvents.Add(domainIntegrationEvent);
    }

    public void RemoveDomainEvent(BaseIntegrationEvent domainIntegrationEvent)
    {
        _domainEvents.Remove(domainIntegrationEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
