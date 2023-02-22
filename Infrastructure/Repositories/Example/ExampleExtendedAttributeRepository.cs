using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Enums.Example;

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

    public async Task<Guid?> Create(ExampleExtendedAttributeCreate createObject)
    {
        var createdId = await _database.SaveDataReturnId(ExampleExtendedAttributes.Insert, createObject);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<ExampleExtendedAttributeDb?> Get(Guid id)
    {
        var foundObject = await _database.LoadData<ExampleExtendedAttributeDb, dynamic>(
            ExampleExtendedAttributes.GetById,
            new {Id = id});
        
        return foundObject.FirstOrDefault();
    }

    public async Task Update(ExampleExtendedAttributeUpdate updateObject)
    {
        await _database.SaveData(ExampleExtendedAttributes.Update, updateObject);
    }

    public async Task Delete(Guid id)
    {
        await _database.SaveData(ExampleExtendedAttributes.Delete, new {Id = id});
    }

    public async Task<List<ExampleExtendedAttributeDb>> GetAttributesForObject(Guid objectId)
    {
        var attributes = await _database.LoadData<ExampleExtendedAttributeDb, dynamic>(
            ExampleExtendedAttributes.GetByAssignedTo, new { AssignedTo = objectId });

        return attributes.ToList();
    }

    public async Task<List<ExampleExtendedAttributeDb>> GetAllOfType(ExampleExtendedAttributeType type)
    {
        var attributes = await _database.LoadData<ExampleExtendedAttributeDb, dynamic>(
            ExampleExtendedAttributes.GetAllOfType, new { Type = type });

        return attributes.ToList();
    }
}