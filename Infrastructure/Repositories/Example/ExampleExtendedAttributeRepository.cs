using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Infrastructure.Repositories.Example;

public class ExampleExtendedAttributeRepository : IExampleExtendedAttributeRepository
{
    private readonly ISqlDataService _database;
    
    public ExampleExtendedAttributeRepository(ISqlDataService database)
    {
        _database = database;
    }
    
    public async Task<List<ExampleExtendedAttributeDb>> GetAll()
    {
        return (await _database.LoadData<ExampleExtendedAttributeDb, dynamic>(ExampleExtendedAttributes.GetAll, new { })).ToList();
    }

    public async Task<Guid> Create(ExampleExtendedAttributeCreate createObject)
    {
        return await _database.SaveDataReturnId(ExampleExtendedAttributes.Insert, createObject);
    }

    public async Task<ExampleExtendedAttributeDb> Get(Guid id)
    {
        var foundObject = await _database.LoadData<ExampleExtendedAttributeDb, dynamic>(
            ExampleExtendedAttributes.GetById,
            new {Id = id});
        
        if (foundObject is null)
            throw new Exception("Id provided was invalid, please try again");
        
        return foundObject.FirstOrDefault()!;
    }

    public async Task Update(ExampleExtendedAttributeUpdate updateObject)
    {
        await _database.SaveData(ExampleExtendedAttributes.Update, updateObject);
    }

    public async Task Delete(Guid id)
    {
        await _database.SaveData(ExampleExtendedAttributes.Delete, new {Id = id});
    }

    public async Task AddToObject(Guid objectId, Guid extendedAttributeId)
    {
        await _database.SaveData(ExampleExAttrJunctions.Insert,
            new {ExampleObjectId = objectId, ExampleExtendedAttributeId = extendedAttributeId});
    }

    public async Task RemoveFromObject(Guid objectId, Guid extendedAttributeId)
    {
        await _database.SaveData(ExampleExAttrJunctions.Delete,
            new {ExampleObjectId = objectId, ExampleExtendedAttributeId = extendedAttributeId});
    }

    public async Task<List<Guid>> GetAttributesForObject(Guid objectId)
    {
        var attributes = await _database.LoadData<Guid, dynamic>(ExampleExAttrJunctions.GetAttributesForObject, new
        {
            ExampleObjectId = objectId
        });

        return attributes.ToList();
    }

    public async Task<List<Guid>> GetObjectsForAttribute(Guid extendedAttributeId)
    {
        var objects = await _database.LoadData<Guid, dynamic>(ExampleExAttrJunctions.GetAttributesForObject, new
        {
            ExampleExtendedAttributeId = extendedAttributeId
        });

        return objects.ToList();
    }

    public async Task<List<ExampleObjectAttributeJunctionFull>> GetAllObjectAttributeMappings()
    {
        var mappings = await _database.LoadData<ExampleObjectAttributeJunctionDb, dynamic>(
            ExampleExAttrJunctions.GetAll, new { });

        var exampleObjects = await _database.LoadData<ExampleObjectDb, dynamic>(
            ExampleObjects.GetAll, new { });

        var exampleAttributes = await _database.LoadData<ExampleExtendedAttributeDb, dynamic>(
            ExampleExtendedAttributes.GetAll, new { });

        var mappingList = mappings.ToList().Select(mapping => new ExampleObjectAttributeJunctionFull()
        {
            ExampleObject = exampleObjects.FirstOrDefault(x => x.Id == mapping.ExampleObjectId)!,
            ExampleAttribute = exampleAttributes.FirstOrDefault(x => x.Id == mapping.ExampleExtendedAttributeId)!
        }).ToList();

        return mappingList;
    }
}