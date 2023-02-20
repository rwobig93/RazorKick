using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Application.Repositories.Example;

public interface IExamplePermissionRepository
{
    Task<List<ExamplePermissionDb>> GetAll();
    Task<Guid?> Create(ExamplePermissionCreate createObject);
    Task<ExamplePermissionDb?> GetById(Guid id);
    Task<ExamplePermissionDb?> GetByName(string permissionName);
    Task<ExamplePermissionDb?> GetByValue(string permissionValue);
    Task Update(ExamplePermissionUpdate objectUpdate);
    Task Delete(Guid id);
    Task AddToObject(Guid objectId, Guid permissionId);
    Task RemoveFromObject(Guid objectId, Guid permissionId);
    Task<List<ExamplePermissionDb>> GetPermissionsForObject(Guid objectId);
    Task<List<ExampleObjectDb>> GetObjectsWithPermission(Guid permissionId);
    Task<List<ExampleObjectPermissionJunctionFull>> GetAllObjectPermissionMappings();
}