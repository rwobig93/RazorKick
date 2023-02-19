using Application.Database;
using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
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
    
    public async Task<List<ExampleObjectDisplay>> GetAll()
    {
        return (await _database.LoadData<ExampleObjectDisplay, dynamic>(ExampleObjects.GetAll, new { })).ToList();
    }

    public async Task CreateRandom()
    {
        PersonNameGenerator generator = new();
        var firstName = generator.GenerateRandomFirstName();
        var lastName = generator.GenerateRandomLastName();

        await Create(firstName, lastName);
    }

    public async Task<Guid?> Create(string firstName, string lastName)
    {
        var newExampleObject = new ExampleObjectCreate()
        {
            FirstName = firstName,
            LastName = lastName
        };
        
        var createdId = await _database.SaveDataReturnId(ExampleObjects.Insert, newExampleObject);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<ExampleObjectDisplay> Get(Guid id)
    {
        var foundObject = await _database.LoadData<ExampleObjectDisplay, dynamic>(
            ExampleObjects.GetById,
            new {Id = id});
        
        if (foundObject is null)
            throw new Exception("Id provided was invalid, please try again");
        
        return foundObject.FirstOrDefault()!;
    }

    public async Task<ExampleObjectDisplayFull> GetFull(Guid id)
    {
        var foundObject = (await _database.LoadData<ExampleObjectDisplayFull, dynamic>(
            ExampleObjects.GetById,
            new {Id = id})).FirstOrDefault();
       
        if (foundObject is null)
            throw new Exception("Id provided was invalid, please try again");

        var foundAttributeMappings = await _extendedAttributeRepository.GetAttributesForObject(foundObject.Id);
        
        foreach (var attributeId in foundAttributeMappings)
            foundObject.ExtendedAttributes.Add(await _extendedAttributeRepository.Get(attributeId));
        
        return foundObject;
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