using Application.Models.Example;
using Domain.Entities.Example;
using Domain.Models.Example;

namespace Application.Repositories.Example;

public interface IExampleExtendedAttributeRepository
{
    Task<List<ExampleExtendedAttributeDb>> GetAll();
    Task<Guid> Create(ExampleExtendedAttributeCreate createObject);
    Task<ExampleExtendedAttributeDb> Get(Guid id);
    Task Update(ExampleExtendedAttributeUpdate objectUpdate);
    Task Delete(Guid id);
    Task AddToObject(Guid objectId, Guid extendedAttributeId);
    Task RemoveFromObject(Guid objectId, Guid extendedAttributeId);
    Task<List<Guid>> GetAttributesForObject(Guid objectId);
    Task<List<Guid>> GetObjectsForAttribute(Guid extendedAttributeId);
    Task<List<ExampleObjectAttributeJunctionFull>> GetAllObjectAttributeMappings();
}