using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Enums.Example;

namespace Application.Repositories.Example;

public interface IExampleExtendedAttributeRepository
{
    Task<List<ExampleExtendedAttributeDb>> GetAll();
    Task<Guid?> Create(ExampleExtendedAttributeCreate createObject);
    Task<ExampleExtendedAttributeDb?> Get(Guid id);
    Task Update(ExampleExtendedAttributeUpdate objectUpdate);
    Task Delete(Guid id);
    Task<List<ExampleExtendedAttributeDb>> GetAttributesForObject(Guid objectId);
    Task<List<ExampleExtendedAttributeDb>> GetAllOfType(ExampleExtendedAttributeType type);
}