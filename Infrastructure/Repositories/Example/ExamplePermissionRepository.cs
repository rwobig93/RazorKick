using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Infrastructure.Repositories.Example;

public class ExamplePermissionRepository : IExamplePermissionRepository
{
    private readonly ISqlDataService _database;
    
    public ExamplePermissionRepository(ISqlDataService database)
    {
        _database = database;
    }
    
    public async Task<List<ExamplePermissionDb>> GetAll()
    {
        return (await _database.LoadData<ExamplePermissionDb, dynamic>(ExamplePermissions.GetAll, new { })).ToList();
    }

    public async Task<Guid?> Create(ExamplePermissionCreate createObject)
    {
        var createdId = await _database.SaveDataReturnId(ExamplePermissions.Insert, createObject);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<ExamplePermissionDb?> GetById(Guid id)
    {
        var foundObject = await _database.LoadData<ExamplePermissionDb, dynamic>(
            ExamplePermissions.GetById,
            new {Id = id});
        
        return foundObject.FirstOrDefault();
    }

    public async Task<ExamplePermissionDb?> GetByName(string permissionName)
    {
        var foundObject = await _database.LoadData<ExamplePermissionDb, dynamic>(
            ExamplePermissions.GetByName,
            new {Name = permissionName});
        
        return foundObject.FirstOrDefault();
    }

    public async Task<ExamplePermissionDb?> GetByValue(string permissionValue)
    {
        var foundObject = await _database.LoadData<ExamplePermissionDb, dynamic>(
            ExamplePermissions.GetByValue,
            new {Value = permissionValue});
        
        return foundObject.FirstOrDefault();
    }

    public async Task Update(ExamplePermissionUpdate updateObject)
    {
        await _database.SaveData(ExamplePermissions.Update, updateObject);
    }

    public async Task Delete(Guid id)
    {
        await _database.SaveData(ExamplePermissions.Delete, new {Id = id});
    }

    public async Task AddToObject(Guid objectId, Guid permissionId)
    {
        await _database.SaveData(ExampleObjectPermissionJunctions.Insert,
            new {ExampleObjectId = objectId, ExamplePermissionId = permissionId});
    }

    public async Task RemoveFromObject(Guid objectId, Guid permissionId)
    {
        await _database.SaveData(ExampleObjectPermissionJunctions.Delete,
            new {ExampleObjectId = objectId, ExamplePermissionId = permissionId});
    }

    public async Task<List<ExamplePermissionDb>> GetPermissionsForObject(Guid objectId)
    {
        var permissions = await _database.LoadData<Guid, dynamic>(
            ExampleObjectPermissionJunctions.GetPermissionsForObject, new { ExampleObjectId = objectId });

        var allPermissions = await GetAll();

        var matchingPermissions = allPermissions.Where(x => permissions.Any(p => p == x.Id));
        
        return matchingPermissions.ToList();
    }

    public async Task<List<ExampleObjectDb>> GetObjectsWithPermission(Guid permissionId)
    {
        var objectsWithPermission = await _database.LoadData<Guid, dynamic>(
            ExampleObjectPermissionJunctions.GetObjectsWithPermission, new { ExamplePermissionId = permissionId });

        var allObjects = await _database.LoadData<ExampleObjectDb, dynamic>(
            ExampleObjects.GetAll, new { });

        var matchingObjects = allObjects.Where(x => objectsWithPermission.Any(p => p == x.Id));
        
        return matchingObjects.ToList();
    }

    public async Task<List<ExampleObjectPermissionJunctionFull>> GetAllObjectPermissionMappings()
    {
        var mappings = await _database.LoadData<ExampleObjectPermissionJunctionDb, dynamic>(
            ExampleObjectPermissionJunctions.GetAll, new { });

        var allObjects = await _database.LoadData<ExampleObjectDb, dynamic>(
            ExampleObjects.GetAll, new { });

        var allPermissions = await _database.LoadData<ExamplePermissionDb, dynamic>(
            ExamplePermissions.GetAll, new { });

        var mappingList = mappings.ToList().Select(mapping => new ExampleObjectPermissionJunctionFull()
        {
            ExampleObject = allObjects.FirstOrDefault(x => x.Id == mapping.ExampleObjectId)!,
            ExamplePermission = allPermissions.FirstOrDefault(x => x.Id == mapping.ExamplePermissionId)!
        }).ToList();

        return mappingList;
    }
}