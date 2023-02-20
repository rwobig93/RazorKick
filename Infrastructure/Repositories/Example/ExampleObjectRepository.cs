using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;
using RandomNameGeneratorNG;

namespace Infrastructure.Repositories.Example;

public class ExampleObjectRepository : IExampleObjectRepository
{
    private readonly ISqlDataService _database;
    private readonly IExampleExtendedAttributeRepository _extendedAttributeRepository;
    
    public ExampleObjectRepository(ISqlDataService database, IExampleExtendedAttributeRepository extendedAttributeRepository)
    {
        _database = database;
        _extendedAttributeRepository = extendedAttributeRepository;
    }
    
    public async Task<List<ExampleObjectDb>> GetAll()
    {
        return (await _database.LoadData<ExampleObjectDb, dynamic>(ExampleObjects.GetAll, new { })).ToList();
    }

    public async Task CreateRandom()
    {
        PersonNameGenerator generator = new();
        var firstName = generator.GenerateRandomFirstName();
        var lastName = generator.GenerateRandomLastName();

        await Create(new ExampleObjectCreate()
        {
            FirstName = firstName,
            LastName = lastName
        });
    }

    public async Task<Guid?> Create(ExampleObjectCreate createObject)
    {
        var createdId = await _database.SaveDataReturnId(ExampleObjects.Insert, createObject);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<ExampleObjectDb?> Get(Guid id)
    {
        var foundObject = await _database.LoadData<ExampleObjectDb, dynamic>(
            ExampleObjects.GetById,
            new {Id = id});
        
        return foundObject.FirstOrDefault()!;
    }

    public async Task<ExampleObjectFull?> GetFull(Guid id)
    {
        var foundObject = (await _database.LoadData<ExampleObjectDb, dynamic>(
            ExampleObjects.GetById,
            new {Id = id})).FirstOrDefault();

        if (foundObject is null)
            return null;

        var foundAttributeMappings = await _extendedAttributeRepository.GetAttributesForObject(foundObject.Id);

        var convertedFullObject = foundObject.ToFullObject();
        
        foreach (var attributeId in foundAttributeMappings)
            convertedFullObject.ExtendedAttributes.Add(await _extendedAttributeRepository.Get(attributeId));
        
        return convertedFullObject;
    }

    public async Task Update(ExampleObjectUpdate updateObject)
    {
        await _database.SaveData(ExampleObjects.Update, updateObject);
    }

    public async Task Delete(Guid id)
    {
        await _database.SaveData(ExampleObjects.Delete, new {Id = id});
    }
}